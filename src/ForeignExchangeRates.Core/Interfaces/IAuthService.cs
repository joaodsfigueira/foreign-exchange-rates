using ForeignExchangeRates.Core.Entities;
using ForeignExchangeRates.Core.Services;

namespace ForeignExchangeRates.Core.Interfaces;

public interface IAuthService
{
	Task<ServiceResult<User>> RegisterAsync(User user, string password);
	Task<ServiceResult<TokenResponse>> LoginAsync(string username, string password);
}
