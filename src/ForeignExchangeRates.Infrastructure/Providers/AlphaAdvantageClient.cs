using AutoMapper;
using ForeignExchangeRates.Core.Entities;
using ForeignExchangeRates.Core.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ForeignExchangeRates.Infrastructure.Providers
{
	public class AlphaAdvantageClient : IThirdPartyRatesProvider
	{
		private HttpClient _httpClient;
		private string _apiKey;
		private readonly IMapper _mapper;

		public AlphaAdvantageClient(HttpClient httpClient, string apiKey, IMapper mapper)
		{
			_httpClient = httpClient;
			_apiKey = apiKey;
			_mapper = mapper;
		}

		public async Task<ExchangeRate?> GetAsync(string sourceCurrencySymbol, string targetCurrencySymbol)
		{
			var query = $"query?function=CURRENCY_EXCHANGE_RATE&from_currency={sourceCurrencySymbol}&to_currency={targetCurrencySymbol}&apikey={_apiKey}";

			var response = await _httpClient.GetAsync(query);
			response.EnsureSuccessStatusCode();

			var resultAsString = await response.Content.ReadAsStringAsync();

			var result = JsonSerializer.Deserialize<AlphaVantageExchangeRateResult>(resultAsString);

			var exchangeResult = _mapper.Map<ExchangeRate>(result?.RealtimeCurrencyExchangeRate);

			return exchangeResult;
		}
	}


	public class AlphaVantageExchangeRateResult
	{
		[JsonPropertyName("Realtime Currency Exchange Rate")]
		public required AlphaVantageRealtimeCurrencyExchangeRate RealtimeCurrencyExchangeRate { get; set; }
	}

	public class AlphaVantageRealtimeCurrencyExchangeRate
	{
		[JsonPropertyName("1. From_Currency Code")]
		public required string SourceCurrencyCode { get; set; }
		[JsonPropertyName("3. To_Currency Code")]
		public required string TargetCurrencyCode { get; set; }
		[JsonPropertyName("5. Exchange Rate")]
		public required string ExchangeRateValue { get; set; }
		[JsonPropertyName("8. Bid Price")]
		public required string BidPrice { get; set; }
		[JsonPropertyName("9. Ask Price")]
		public required string AskPrice { get; set; }
	}
}
