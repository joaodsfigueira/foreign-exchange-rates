using ForeignExchangeRates.Core.Entities;

namespace ForeignExchangeRates.Core.Interfaces;

public interface IExchangeRateRepository : IGenericRepository<ExchangeRate>
{
	Task<ExchangeRate?> GetAsync(string sourceCurrencyCode, string targetCurrencyCode);
}
