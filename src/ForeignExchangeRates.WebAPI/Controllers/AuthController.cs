using AutoMapper;
using ForeignExchangeRates.Core.Entities;
using ForeignExchangeRates.Core.Interfaces;
using ForeignExchangeRates.Core.Services;
using ForeignExchangeRates.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ForeignExchangeRates.WebAPI.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly IAuthService _authService;
	private readonly IMapper _mapper;

	public AuthController(IAuthService authService, IMapper mapper)
	{
		_authService = authService;
		_mapper = mapper;
	}

	[ProducesResponseType(typeof(ExchangeRateDto), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(IDictionary<string, IEnumerable<string>>), (int)HttpStatusCode.BadRequest)]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
	[HttpPost("register")]
	public async Task<ActionResult<UserDto>> Register(UserPostModel userPostModel)
	{
		var user = _mapper.Map<User>(userPostModel);
		var serviceResult = await _authService.RegisterAsync(user, userPostModel.Password);
		switch (serviceResult)
		{
			case SuccessResult<User> successResult:
				return Ok(_mapper.Map<UserDto>(successResult.ResultObject));
			case ValidationFailedResult<User> validationFailedResult:
				return BadRequest(validationFailedResult.Errors);
			case ConflictResult<User>:
				return Conflict("Username already exists");
			default:
				throw new InvalidOperationException("Service result not expected");
		}
	}

	[ProducesResponseType(typeof(ExchangeRateDto), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(IDictionary<string, IEnumerable<string>>), (int)HttpStatusCode.BadRequest)]
	[HttpPost("login")]
	public async Task<ActionResult<TokenResponseDto>> Login(UserPostModel userLoginModel)
	{
		var serviceResult = await _authService.LoginAsync(userLoginModel.Username, userLoginModel.Password);
		switch (serviceResult)
		{
			case SuccessResult<TokenResponse> successResult:
				return Ok(_mapper.Map<TokenResponseDto>(successResult.ResultObject));
			case ValidationFailedResult<TokenResponse> validationFailedResult:
				return BadRequest(validationFailedResult.Errors);
			default:
				throw new InvalidOperationException("Service result not expected");
		}
	}
}
