using ForeignExchangeRates.Core.Interfaces;

namespace ForeignExchangeRates.Infrastructure.Database;

public class UnitOfWork : IUnitOfWork
{
	private readonly AppDbContext _context;

	public UnitOfWork(AppDbContext context)
	{
		_context = context;
	}

	public void Dispose()
	{
		_context.Dispose();
	}

	public async Task SaveChangesAsync()
	{
		await _context.SaveChangesAsync();
	}
}
