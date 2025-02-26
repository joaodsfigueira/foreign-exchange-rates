using ForeignExchangeRates.Core.Entities;
using ForeignExchangeRates.Core.Services;

namespace ForeignExchangeRates.Core.Interfaces;

public interface IExchangeRateService
{
	Task<ExchangeRate?> GetAsync(string sourceCurrencyCode, string targetCurrencyCode);
	Task<ServiceResult<ExchangeRate>> CreateAsync(ExchangeRate exchangeRate);
	Task<ServiceResult<ExchangeRate>> UpdateAsync(ExchangeRate exchangeRate, bool validateExistence = true);
	Task<ServiceResult<ExchangeRate>> DeleteAsync(ExchangeRate exchangeRate, bool validateExistence = true);
}
