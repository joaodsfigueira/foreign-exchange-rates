using ForeignExchangeRates.Core.Entities;
using ForeignExchangeRates.Core.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ForeignExchangeRates.WebAPI.Generators;

public class TokenGenerator : ITokenGenerator
{
	private readonly IConfiguration _configuration;

	public TokenGenerator(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public string CreateToken(User user)
	{
		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Name, user.Username),
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
		};

		var key = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Authentication:Token")!));

		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

		var tokenDescriptor = new JwtSecurityToken(
			issuer: _configuration.GetValue<string>("Authentication:Issuer"),
			audience: _configuration.GetValue<string>("Authentication:Audience"),
			claims: claims,
			expires: DateTime.UtcNow.AddDays(1),
			signingCredentials: creds
		);

		return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
	}
}