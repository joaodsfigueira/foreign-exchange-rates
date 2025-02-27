using ForeignExchangeRates.Core.Entities;
using ForeignExchangeRates.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ForeignExchangeRates.Core.Services;

public class AuthService : IAuthService
{
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ITokenGenerator _tokenGenerator;

	public AuthService(IUserRepository userRepository, IUnitOfWork unitOfWork,
		ITokenGenerator tokenGenerator)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
		_tokenGenerator = tokenGenerator;
	}

	public async Task<ServiceResult<TokenResponse>> LoginAsync(string username, string password)
	{
		var user = await _userRepository.GetAsync(username);
		if (user is null)
		{
			return ServiceResult<TokenResponse>.NotFound();
		}

		if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, password)
						== PasswordVerificationResult.Failed)
		{
			return ServiceResult<TokenResponse>.ValidationFailed(new Dictionary<string, string[]>(){
				["Password"] = ["Invalid password"]
			});
		}

		var token = _tokenGenerator.CreateToken(user);

		return ServiceResult<TokenResponse>.Success(new TokenResponse(){
			AccessToken = token
		});
	}

	public async Task<ServiceResult<User>> RegisterAsync(User user, string password)
	{
		var userFound = await _userRepository.GetAsync(user.Username);
		if (userFound is not null)
		{
			return ServiceResult<User>.Conflict();
		}

		user.PasswordHash = new PasswordHasher<User>()
			.HashPassword(user, password);

		await _userRepository.InsertAsync(user);

		await _unitOfWork.SaveChangesAsync();

		return ServiceResult<User>.Success(user);
	}

	
}
