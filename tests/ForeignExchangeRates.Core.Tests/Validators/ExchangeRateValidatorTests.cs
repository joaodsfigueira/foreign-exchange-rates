using ForeignExchangeRates.Core.Entities;
using ForeignExchangeRates.Core.Validators;

namespace ForeignExchangeRates.Core.Tests.Validators;

public class ExchangeRateValidatorTests
{
	public ExchangeRateValidator _exchangeRateValidator;

	public ExchangeRateValidatorTests()
	{
		_exchangeRateValidator = new ExchangeRateValidator();
	}

	[Fact]
	public async Task InvalidAskPrice()
	{
		var exchangeRate = new ExchangeRate()
		{
			Id = Guid.Empty,
			SourceCurrencyCode = "EUR",
			TargetCurrencyCode = "USD",
			ExchangeRateValue = 1,
			AskPrice = -1,
			BidPrice = 1
		};

		var validationResult = await _exchangeRateValidator.ValidateAsync(exchangeRate);
		Assert.False(validationResult.IsValid);
		Assert.Contains(nameof(exchangeRate.AskPrice), validationResult.Errors.Select(e => e.PropertyName));
	}

	[Fact]
	public async Task InvalidBidPrice()
	{
		var exchangeRate = new ExchangeRate()
		{
			Id = Guid.Empty,
			SourceCurrencyCode = "EUR",
			TargetCurrencyCode = "USD",
			ExchangeRateValue = 1,
			AskPrice = 1,
			BidPrice = -1
		};

		var validationResult = await _exchangeRateValidator.ValidateAsync(exchangeRate);
		Assert.False(validationResult.IsValid);
		Assert.Contains(nameof(exchangeRate.BidPrice), validationResult.Errors.Select(e => e.PropertyName));
	}

	[Fact]
	public async Task InvalidExchangeRateValue()
	{
		var exchangeRate = new ExchangeRate()
		{
			Id = Guid.Empty,
			SourceCurrencyCode = "EUR",
			TargetCurrencyCode = "USD",
			ExchangeRateValue = -1,
			AskPrice = 1,
			BidPrice = 1
		};

		var validationResult = await _exchangeRateValidator.ValidateAsync(exchangeRate);
		Assert.False(validationResult.IsValid);
		Assert.Contains(nameof(exchangeRate.ExchangeRateValue), validationResult.Errors.Select(e => e.PropertyName));
	}

	[Fact]
	public async Task InvalidSourceCurrencyCode_MoreCharactersThanAllowed()
	{
		var exchangeRate = new ExchangeRate()
		{
			Id = Guid.Empty,
			SourceCurrencyCode = "EURO",
			TargetCurrencyCode = "USD",
			ExchangeRateValue = 1,
			AskPrice = 1,
			BidPrice = 1
		};

		var validationResult = await _exchangeRateValidator.ValidateAsync(exchangeRate);
		Assert.False(validationResult.IsValid);
		Assert.Contains(nameof(exchangeRate.SourceCurrencyCode), validationResult.Errors.Select(e => e.PropertyName));
	}

	[Fact]
	public async Task InvalidSourceCurrencyCode_Lowercase()
	{
		var exchangeRate = new ExchangeRate()
		{
			Id = Guid.Empty,
			SourceCurrencyCode = "eur",
			TargetCurrencyCode = "USD",
			ExchangeRateValue = 1,
			AskPrice = 1,
			BidPrice = 1
		};

		var validationResult = await _exchangeRateValidator.ValidateAsync(exchangeRate);
		Assert.False(validationResult.IsValid);
		Assert.Contains(nameof(exchangeRate.SourceCurrencyCode), validationResult.Errors.Select(e => e.PropertyName));
	}

	[Fact]
	public async Task InvalidTargetCurrencyCode_MoreCharactersThanAllowed()
	{
		var exchangeRate = new ExchangeRate()
		{
			Id = Guid.Empty,
			SourceCurrencyCode = "EUR",
			TargetCurrencyCode = "USDD",
			ExchangeRateValue = 1,
			AskPrice = 1,
			BidPrice = 1
		};

		var validationResult = await _exchangeRateValidator.ValidateAsync(exchangeRate);
		Assert.False(validationResult.IsValid);
		Assert.Contains(nameof(exchangeRate.TargetCurrencyCode), validationResult.Errors.Select(e => e.PropertyName));
	}

	[Fact]
	public async Task InvalidTargetCurrencyCode_Lowercase()
	{
		var exchangeRate = new ExchangeRate()
		{
			Id = Guid.Empty,
			SourceCurrencyCode = "EUR",
			TargetCurrencyCode = "usd",
			ExchangeRateValue = 1,
			AskPrice = 1,
			BidPrice = 1
		};

		var validationResult = await _exchangeRateValidator.ValidateAsync(exchangeRate);
		Assert.False(validationResult.IsValid);
		Assert.Contains(nameof(exchangeRate.TargetCurrencyCode), validationResult.Errors.Select(e => e.PropertyName));
	}

	[Fact]
	public async Task ValidExchangeRate()
	{
		var exchangeRate = new ExchangeRate()
		{
			Id = Guid.NewGuid(),
			SourceCurrencyCode = "EUR",
			TargetCurrencyCode = "USD",
			ExchangeRateValue = 1,
			AskPrice = 1,
			BidPrice = 1
		};

		var validationResult = await _exchangeRateValidator.ValidateAsync(exchangeRate);
		Assert.True(validationResult.IsValid);
	}
}
