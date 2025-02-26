using ForeignExchangeRates.Core.Entities;

namespace ForeignExchangeRates.Core.Interfaces
{
    public interface IThirdPartyRatesProvider
    {
		Task<ExchangeRate?> GetAsync(string sourceCurrencyCode, string targetCurrencyCode);
	}   
}
