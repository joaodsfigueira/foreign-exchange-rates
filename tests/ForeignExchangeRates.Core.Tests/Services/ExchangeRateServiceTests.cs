using FluentValidation;
using FluentValidation.Results;
using ForeignExchangeRates.Core.Entities;
using ForeignExchangeRates.Core.Interfaces;
using ForeignExchangeRates.Core.Services;
using Moq;

namespace ForeignExchangeRates.Core.Tests.Services;

public class ExchangeRateServiceTests
{
	private readonly Mock<IExchangeRateRepository> _exchangeRateRepositoryMock;
	private readonly Mock<IThirdPartyRatesProvider> _thirdPartyRatesProviderMock;
	private readonly Mock<IUnitOfWork> _unitOfWorkMock;
	private readonly Mock<IEventSourcingProvider> _eventSourcingProviderMock;
	private readonly Mock<IValidator<ExchangeRate>> _validatorMock;

	private readonly ExchangeRateService _exchangeRateService;

	public ExchangeRateServiceTests() 
	{
		_exchangeRateRepositoryMock = new Mock<IExchangeRateRepository>();
		_thirdPartyRatesProviderMock = new Mock<IThirdPartyRatesProvider>();
		_unitOfWorkMock = new Mock<IUnitOfWork>();
		_eventSourcingProviderMock = new Mock<IEventSourcingProvider>();
		_validatorMock = new Mock<IValidator<ExchangeRate>>();
		_exchangeRateService = new ExchangeRateService(_exchangeRateRepositoryMock.Object,
			_thirdPartyRatesProviderMock.Object, _unitOfWorkMock.Object, _eventSourcingProviderMock.Object,
			_validatorMock.Object);
	}
	
	[Fact]
	public async Task Get_FetchFromThirdPartyIfNotFoundAndSave()
	{
		var sourceCurrencyCode = "EUR";
		var targetCurrencyCode = "USD";
		var exchangeRate = new ExchangeRate()
		{
			Id = Guid.Empty,
			SourceCurrencyCode = sourceCurrencyCode,
			TargetCurrencyCode = targetCurrencyCode,
			ExchangeRateValue = 1,
			AskPrice = -1,
			BidPrice = 1
		};
		_exchangeRateRepositoryMock.Setup(s => s.GetAsync(sourceCurrencyCode, targetCurrencyCode)).ReturnsAsync((ExchangeRate?)null);
		_thirdPartyRatesProviderMock.Setup(s => s.GetAsync(sourceCurrencyCode, targetCurrencyCode)).ReturnsAsync(exchangeRate);
		_validatorMock.Setup(s => s.ValidateAsync(exchangeRate, CancellationToken.None)).ReturnsAsync(new ValidationResult());
		_exchangeRateRepositoryMock.Setup(s => s.InsertAsync(exchangeRate));
		_unitOfWorkMock.Setup(s => s.SaveChangesAsync());
		_eventSourcingProviderMock.Setup(s => s.SaveEventAsync(It.IsAny<EventMessage>()));
		var result = await _exchangeRateService.GetAsync(sourceCurrencyCode, targetCurrencyCode);
		_exchangeRateRepositoryMock.VerifyAll();
		_thirdPartyRatesProviderMock.VerifyAll();
		_validatorMock.VerifyAll();
		_unitOfWorkMock.VerifyAll();
		_eventSourcingProviderMock.VerifyAll();
		Assert.IsType<ExchangeRate>(result);
		Assert.Equal(result, exchangeRate);
	}

	[Fact]
	public async Task Get_FoundOnDb()
	{
		var sourceCurrencyCode = "EUR";
		var targetCurrencyCode = "USD";
		var exchangeRate = new ExchangeRate()
		{
			Id = Guid.Empty,
			SourceCurrencyCode = sourceCurrencyCode,
			TargetCurrencyCode = targetCurrencyCode,
			ExchangeRateValue = 1,
			AskPrice = -1,
			BidPrice = 1
		};
		_exchangeRateRepositoryMock.Setup(s => s.GetAsync(sourceCurrencyCode, targetCurrencyCode)).ReturnsAsync(exchangeRate);
		var result = await _exchangeRateService.GetAsync(sourceCurrencyCode, targetCurrencyCode);
		_exchangeRateRepositoryMock.VerifyAll();
		Assert.IsType<ExchangeRate>(result);
		Assert.Equal(result, exchangeRate);
	}
}
