using ForeignExchangeRates.Core.Entities;

namespace ForeignExchangeRates.Core.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
	Task<User?> GetAsync(string username);
}
