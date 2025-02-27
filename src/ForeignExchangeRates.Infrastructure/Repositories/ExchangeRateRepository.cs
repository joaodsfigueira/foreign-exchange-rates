using ForeignExchangeRates.Core.Entities;
using ForeignExchangeRates.Core.Interfaces;
using ForeignExchangeRates.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ForeignExchangeRates.Infrastructure.Repositories
{
	public class ExchangeRateRepository : IExchangeRateRepository
	{
		private readonly AppDbContext _dbContext;

		public ExchangeRateRepository(AppDbContext dbContext)
		{
			_dbContext = dbContext;			
		}

		public async Task<ExchangeRate?> GetAsync(string sourceCurrencyCode, string targetCurrencyCode)
		{
			return await _dbContext.ExchangeRates.FirstOrDefaultAsync(er => er.SourceCurrencyCode == sourceCurrencyCode && er.TargetCurrencyCode == targetCurrencyCode);
		}

		public async Task InsertAsync(ExchangeRate exchangeRate)
		{
			await _dbContext.AddAsync(exchangeRate);
		}

		public void Update(ExchangeRate exchangeRate)
		{
			_dbContext.Update(exchangeRate);
		}

		public void Delete(ExchangeRate exchangeRate)
		{
			_dbContext.Remove(exchangeRate);
		}
	}
}
