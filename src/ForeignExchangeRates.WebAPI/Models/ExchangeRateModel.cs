namespace ForeignExchangeRates.WebAPI.Models;

public class ExchangeRatePostModel
{
	public required decimal ExchangeRateValue { get; set; }
	public required decimal BidPrice { get; set; }
	public required decimal AskPrice { get; set; }
}

public class ExchangeRatePutModel
{
	public required decimal ExchangeRateValue { get; set; }
	public required decimal BidPrice { get; set; }
	public required decimal AskPrice { get; set; }
}

public class ExchangeRateDto
{
	public required string SourceCurrencyCode { get; set; }
	public required string TargetCurrencyCode { get; set; }
	public required decimal ExchangeRateValue { get; set; }
	public required decimal BidPrice { get; set; }
	public required decimal AskPrice { get; set; }
}
