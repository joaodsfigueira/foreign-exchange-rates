using Microsoft.Net.Http.Headers;
using System.Text;

namespace ForeignExchangeRates.WebAPI.Middlewares;

public class ApiKeyMiddleware
{
	private readonly RequestDelegate _next;
	private readonly string _apiKey;

	public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
	{
		_next = next;
		_apiKey = configuration["ApiKey"] ?? throw new InvalidOperationException("ApiKey configuration is invalid.");
	}

	public async Task InvokeAsync(HttpContext context)
	{
		if (!string.Equals(context.Request.Headers["ApiKey"].FirstOrDefault(), _apiKey))
		{
			context.Response.StatusCode = 403;
			await context.Response.WriteAsync("Invalid api key. Please include a valid key on a ApiKey http header.", Encoding.UTF8);
		}
		else
		{
			await _next(context);
		}
	}
}
