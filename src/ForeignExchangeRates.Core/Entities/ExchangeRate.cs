using System.ComponentModel.DataAnnotations;

namespace ForeignExchangeRates.Core.Entities;

public class ExchangeRate
{
    [Key]
    public required Guid Id { get; set; }
    public required string SourceCurrencyCode { get; set; }
    public required string TargetCurrencyCode { get; set; }
    public required decimal ExchangeRateValue { get; set; }
    public required decimal BidPrice { get; set; }
    public required decimal AskPrice { get; set; }
}
