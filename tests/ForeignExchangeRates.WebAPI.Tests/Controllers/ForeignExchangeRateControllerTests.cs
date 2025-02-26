using AutoMapper;
using ForeignExchangeRates.Core.Entities;
using ForeignExchangeRates.Core.Interfaces;
using ForeignExchangeRates.Core.Services;
using ForeignExchangeRates.WebAPI.Automapper;
using ForeignExchangeRates.WebAPI.Controllers;
using ForeignExchangeRates.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ForeignExchangeRates.WebAPI.Tests.Controllers;

public class ForeignExchangeRateControllerTests
{
	private Mock<IExchangeRateRepository> _exchangeRateRepositoryMock;
	private Mock<IExchangeRateService> _exchangeRateServiceMock;
	
	private IMapper _mapper;
	private ExchangeRateController _exchangeRateController;

	public ForeignExchangeRateControllerTests()
	{
		_exchangeRateRepositoryMock = new Mock<IExchangeRateRepository>();
		_exchangeRateServiceMock = new Mock<IExchangeRateService>();
		var modelsProfile = new ModelsProfile();
		var configuration = new MapperConfiguration(cfg => cfg.AddProfile(modelsProfile));
		_mapper = new Mapper(configuration);

		_exchangeRateController = new ExchangeRateController(_exchangeRateRepositoryMock.Object,
			_exchangeRateServiceMock.Object, _mapper);
	}

	[Fact]
	public async Task Get_ShouldReturnNotFound()
	{
		var sourceCurrencyCode = "EUR";
		var targetCurrencyCode = "USD";
		_exchangeRateServiceMock.Setup(s => s.GetAsync(sourceCurrencyCode, targetCurrencyCode)).ReturnsAsync((ExchangeRate?)null);
		var actionResult = await _exchangeRateController.GetAsync(sourceCurrencyCode, targetCurrencyCode);
		Assert.IsType<NotFoundResult>(actionResult);
		_exchangeRateServiceMock.VerifyAll();
	}

	[Fact]
	public async Task Get_ShouldReturnExchangeRateDto()
	{
		var sourceCurrencyCode = "EUR";
		var targetCurrencyCode = "USD";
		var exchangeRate = new ExchangeRate()
		{
			Id = Guid.Empty,
			SourceCurrencyCode = sourceCurrencyCode,
			TargetCurrencyCode = targetCurrencyCode,
			ExchangeRateValue = 0,
			AskPrice = 0,
			BidPrice = 0
		};
		_exchangeRateServiceMock.Setup(s => s.GetAsync(sourceCurrencyCode, targetCurrencyCode)).ReturnsAsync(exchangeRate);
		var actionResult = await _exchangeRateController.GetAsync(sourceCurrencyCode, targetCurrencyCode);
		var okResult = Assert.IsType<OkObjectResult>(actionResult);
		var exchangeRateDto = Assert.IsType<ExchangeRateDto>(okResult.Value);
		Assert.Equal(exchangeRate.SourceCurrencyCode, exchangeRateDto.SourceCurrencyCode);
		Assert.Equal(exchangeRate.TargetCurrencyCode, exchangeRateDto.TargetCurrencyCode);
		Assert.Equal(exchangeRate.ExchangeRateValue, exchangeRateDto.ExchangeRateValue);
		Assert.Equal(exchangeRate.BidPrice, exchangeRateDto.BidPrice);
		Assert.Equal(exchangeRate.AskPrice, exchangeRateDto.AskPrice);
	}

	[Fact]
	public async Task Insert_ShouldReturnConflictResult()
	{
		var sourceCurrencyCode = "EUR";
		var targetCurrencyCode = "USD";
		var postModel = new ExchangeRatePostModel()
		{
			ExchangeRateValue = 1,
			AskPrice = 1,
			BidPrice = 1
		};
		_exchangeRateServiceMock.Setup(s => s.CreateAsync(It.IsAny<ExchangeRate>())).ReturnsAsync(ServiceResult<ExchangeRate>.Conflict());
		var actionResult = await _exchangeRateController.InsertAsync(sourceCurrencyCode, targetCurrencyCode, postModel);
		Assert.IsType<ConflictResult>(actionResult);
	}

	[Fact]
	public async Task Insert_ShouldReturnBadRequestResult()
	{
		var sourceCurrencyCode = "EUR";
		var targetCurrencyCode = "USD";
		var postModel = new ExchangeRatePostModel()
		{
			ExchangeRateValue = 1,
			AskPrice = 1,
			BidPrice = 1
		};
		var errors = new Dictionary<string, string[]>();
		_exchangeRateServiceMock.Setup(s => s.CreateAsync(It.IsAny<ExchangeRate>())).ReturnsAsync(ServiceResult<ExchangeRate>.ValidationFailed(errors));
		var actionResult = await _exchangeRateController.InsertAsync(sourceCurrencyCode, targetCurrencyCode, postModel);
		Assert.IsType<BadRequestObjectResult>(actionResult);
		_exchangeRateServiceMock.VerifyAll();
	}

	[Fact]
	public async Task Insert_ShouldCreateSuccessfully()
	{
		var sourceCurrencyCode = "EUR";
		var targetCurrencyCode = "USD";
		var postModel = new ExchangeRatePostModel()
		{
			ExchangeRateValue = 1,
			AskPrice = 1,
			BidPrice = 1
		};
		var errors = new Dictionary<string, string[]>();
		var exchangeRate = new ExchangeRate()
		{
			Id = Guid.Empty,
			SourceCurrencyCode = sourceCurrencyCode,
			TargetCurrencyCode = targetCurrencyCode,
			ExchangeRateValue = 1,
			AskPrice = 1,
			BidPrice = 1
		};
		_exchangeRateServiceMock.Setup(s => s.CreateAsync(It.IsAny<ExchangeRate>())).ReturnsAsync(ServiceResult<ExchangeRate>.Success(exchangeRate));
		var actionResult = await _exchangeRateController.InsertAsync(sourceCurrencyCode, targetCurrencyCode, postModel);
		var okResult = Assert.IsType<OkObjectResult>(actionResult);
		var exchangeRateDto = Assert.IsType<ExchangeRateDto>(okResult.Value);
		Assert.Equal(exchangeRate.SourceCurrencyCode, exchangeRateDto.SourceCurrencyCode);
		Assert.Equal(exchangeRate.TargetCurrencyCode, exchangeRateDto.TargetCurrencyCode);
		Assert.Equal(exchangeRate.ExchangeRateValue, exchangeRateDto.ExchangeRateValue);
		Assert.Equal(exchangeRate.BidPrice, exchangeRateDto.BidPrice);
		Assert.Equal(exchangeRate.AskPrice, exchangeRateDto.AskPrice);
	}

	[Fact]
	public async Task Update_ShouldReturnNotFound()
	{
		var sourceCurrencyCode = "EUR";
		var targetCurrencyCode = "USD";
		var putModel = new ExchangeRatePutModel()
		{
			ExchangeRateValue = 1,
			AskPrice = 1,
			BidPrice = 1
		};
		_exchangeRateRepositoryMock.Setup(s => s.GetAsync(sourceCurrencyCode, targetCurrencyCode)).ReturnsAsync((ExchangeRate?)null);
		var actionResult = await _exchangeRateController.UpdateAsync(sourceCurrencyCode, targetCurrencyCode, putModel);
		Assert.IsType<NotFoundResult>(actionResult);
		_exchangeRateRepositoryMock.VerifyAll();
	}

	[Fact]
	public async Task Update_ShouldReturnBadRequestResult()
	{
		var sourceCurrencyCode = "EUR";
		var targetCurrencyCode = "USD";
		var putModel = new ExchangeRatePutModel()
		{
			ExchangeRateValue = 1,
			AskPrice = 1,
			BidPrice = 1
		};
		var errors = new Dictionary<string, string[]>();
		var exchangeRate = new ExchangeRate()
		{
			Id = Guid.Empty,
			SourceCurrencyCode = sourceCurrencyCode,
			TargetCurrencyCode = targetCurrencyCode,
			ExchangeRateValue = 1,
			AskPrice = 1,
			BidPrice = 1
		};
		_exchangeRateRepositoryMock.Setup(s => s.GetAsync(sourceCurrencyCode, targetCurrencyCode)).ReturnsAsync(exchangeRate);
		_exchangeRateServiceMock.Setup(s => s.UpdateAsync(It.IsAny<ExchangeRate>(), false)).ReturnsAsync(ServiceResult<ExchangeRate>.ValidationFailed(errors));
		var actionResult = await _exchangeRateController.UpdateAsync(sourceCurrencyCode, targetCurrencyCode, putModel);
		_exchangeRateRepositoryMock.VerifyAll();
		_exchangeRateServiceMock.VerifyAll();
		Assert.IsType<BadRequestObjectResult>(actionResult);
	}

	[Fact]
	public async Task Update_ShouldUpdateSuccessfully()
	{
		var sourceCurrencyCode = "EUR";
		var targetCurrencyCode = "USD";
		var putModel = new ExchangeRatePutModel()
		{
			ExchangeRateValue = 1,
			AskPrice = 1,
			BidPrice = 1
		};
		var errors = new Dictionary<string, string[]>();
		var exchangeRate = new ExchangeRate()
		{
			Id = Guid.Empty,
			SourceCurrencyCode = sourceCurrencyCode,
			TargetCurrencyCode = targetCurrencyCode,
			ExchangeRateValue = 1,
			AskPrice = 1,
			BidPrice = 1
		};
		_exchangeRateRepositoryMock.Setup(s => s.GetAsync(sourceCurrencyCode, targetCurrencyCode)).ReturnsAsync(exchangeRate);
		_exchangeRateServiceMock.Setup(s => s.UpdateAsync(It.IsAny<ExchangeRate>(), false)).ReturnsAsync(ServiceResult<ExchangeRate>.Success(exchangeRate));
		var actionResult = await _exchangeRateController.UpdateAsync(sourceCurrencyCode, targetCurrencyCode, putModel);
		var okResult = Assert.IsType<OkObjectResult>(actionResult);
		var exchangeRateDto = Assert.IsType<ExchangeRateDto>(okResult.Value);
		Assert.Equal(exchangeRate.SourceCurrencyCode, exchangeRateDto.SourceCurrencyCode);
		Assert.Equal(exchangeRate.TargetCurrencyCode, exchangeRateDto.TargetCurrencyCode);
		Assert.Equal(exchangeRate.ExchangeRateValue, exchangeRateDto.ExchangeRateValue);
		Assert.Equal(exchangeRate.BidPrice, exchangeRateDto.BidPrice);
		Assert.Equal(exchangeRate.AskPrice, exchangeRateDto.AskPrice);
	}
}
