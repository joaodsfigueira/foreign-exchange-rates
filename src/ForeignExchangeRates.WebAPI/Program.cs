using ForeignExchangeRates.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ForeignExchangeRates.Core.Interfaces;
using ForeignExchangeRates.Infrastructure.Providers;
using FluentValidation;
using ForeignExchangeRates.Core.Entities;
using ForeignExchangeRates.Core.Validators;
using Azure.Messaging.ServiceBus;
using ForeignExchangeRates.Infrastructure.Repositories;
using ForeignExchangeRates.Core.Services;
using AutoMapper;
using ForeignExchangeRates.WebAPI.Automapper;
using ForeignExchangeRates.Infrastructure.Automapper;
using ForeignExchangeRates.WebAPI.Polly;
using ForeignExchangeRates.WebAPI.Generators;
using Microsoft.OpenApi.Models;
using ForeignExchangeRates.WebAPI.OperationFilters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ForeignExchangeRates.WebAPI;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddApiVersioning(options =>
		{
			options.DefaultApiVersion = new ApiVersion(1, 0);
			options.AssumeDefaultVersionWhenUnspecified = true;
			options.ReportApiVersions = true;
			options.ApiVersionReader = new UrlSegmentApiVersionReader();
		});

		if (bool.TryParse(builder.Configuration["Database:UseInMemory"], out bool useInMemoryDatabase) && useInMemoryDatabase)
		{
			builder.Services.AddDbContext<AppDbContext>(opt =>
						opt.UseInMemoryDatabase("ForeignExchangeRates"));
		}
		else 
		{
			var connectionString =
				builder.Configuration["Database:ConnectionString"]
					?? throw new InvalidOperationException("Database:ConnectionString string not found.");

			builder.Services.AddDbContext<AppDbContext>(opt =>
				opt.UseSqlServer(connectionString));
		}

		builder.Services.AddAutoMapper(ops => {
			ops.AddProfile<ModelsProfile>();
			ops.AddProfile<AlphaVantageProfile>();
		});

		builder.Services.AddScoped<IAuthService, AuthService>();
		builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
		builder.Services.AddScoped<IUserRepository, UserRepository>();
		builder.Services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
		builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
		builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
		builder.Services.AddScoped<IValidator<ExchangeRate>, ExchangeRateValidator>();
		builder.Services.AddHttpClient<IThirdPartyRatesProvider, AlphaAdvantageClient>((client, sp) =>
		{
			var apiKey = builder.Configuration["ThirdPartyProvider:ApiKey"] ??
							throw new InvalidOperationException("ThirdPartyProvider:ApiKey not found.");
			client.BaseAddress = new Uri(builder.Configuration["ThirdPartyProvider:BaseUrl"]
				?? throw new InvalidOperationException("ThirdPartyProvider:BaseUrl not found."));
			return new AlphaAdvantageClient(client, apiKey, sp.GetRequiredService<IMapper>());
		}).AddPolicyHandler(RetryPolicyProvider.GetRetryPolicy());

		if (bool.TryParse(builder.Configuration["EventSourcing:Use"], out bool useEventSourcing) && useEventSourcing)
		{
			var queueName = builder.Configuration["EventSourcing:QueueName"]
							?? throw new InvalidOperationException("EventSourcing:QueueName not found.");
			builder.Services.AddScoped(c => new ServiceBusClient(builder.Configuration["EventSourcing:AzureServiceBus:ConnectionString"]
				?? throw new InvalidOperationException("EventSourcing:AzureServiceBus:ConnectionString not found.")));
			builder.Services.AddScoped<IEventSourcingProvider, AzureServiceBusProvider>(c => new AzureServiceBusProvider(c.GetRequiredService<ServiceBusClient>(), queueName));
		}
		else
		{
			builder.Services.AddScoped<IEventSourcingProvider, NullEventSourcingProvider>();
		}
		
		builder.Services.AddControllers();

		builder.Services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo { Title = "Foreign Exchange Rates API", Version = "v1" });
			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
			{
				Name = "Authorization",
				Type = SecuritySchemeType.ApiKey,
				Scheme = "Bearer",
				BearerFormat = "JWT",
				In = ParameterLocation.Header,
				Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
			});
			c.AddSecurityRequirement(new OpenApiSecurityRequirement {
				{
					new OpenApiSecurityScheme {
						Reference = new OpenApiReference {
							Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
						}
					},
					new string[] {}
				}
			});
		});

		builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = builder.Configuration["Authentication:Issuer"],
					ValidateAudience = true,
					ValidAudience = builder.Configuration["Authentication:Audience"],
					ValidateLifetime = true,
					IssuerSigningKey = new SymmetricSecurityKey(
						Encoding.UTF8.GetBytes(builder.Configuration["Authentication:Token"]!)),
					ValidateIssuerSigningKey = true
				};
			});
		

		var app = builder.Build();

		app.UseSwagger();
		app.UseSwaggerUI();

		app.UseHttpsRedirection();

		app.UseAuthentication();

		app.UseAuthorization();

		app.MapControllers();

		app.Run();
	}
}
