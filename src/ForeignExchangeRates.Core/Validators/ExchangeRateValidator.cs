using FluentValidation;
using ForeignExchangeRates.Core.Entities;
using System.Text.RegularExpressions;

namespace ForeignExchangeRates.Core.Validators;

public class ExchangeRateValidator : AbstractValidator<ExchangeRate>
{
	public ExchangeRateValidator()
	{
		var regexForCodes = new Regex(@"^[A-Z]{3}$", RegexOptions.Compiled);

		RuleFor(er => er.SourceCurrencyCode)
			.NotEmpty()
			.Matches(regexForCodes);
		RuleFor(er => er.TargetCurrencyCode)
			.NotEmpty()
			.Matches(regexForCodes);
		RuleFor(er => er.ExchangeRateValue)
			.NotNull()
			.GreaterThanOrEqualTo(0);
		RuleFor(er => er.AskPrice)
			.NotNull()
			.GreaterThanOrEqualTo(0);
		RuleFor(er => er.BidPrice)
			.NotNull()
			.GreaterThanOrEqualTo(0);
	}
}
