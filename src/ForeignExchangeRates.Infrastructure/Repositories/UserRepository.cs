using ForeignExchangeRates.Core.Entities;
using ForeignExchangeRates.Core.Interfaces;
using ForeignExchangeRates.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ForeignExchangeRates.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
	private readonly AppDbContext _dbContext;

	public UserRepository(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}
	public async Task<User?> GetAsync(string username)
	{
		return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
	}

	public async Task InsertAsync(User user)
	{
		await _dbContext.AddAsync(user);
	}

	public void Update(User user)
	{
		_dbContext.Update(user);
	}

	public void Delete(User user)
	{
		_dbContext.Remove(user);
	}
}
