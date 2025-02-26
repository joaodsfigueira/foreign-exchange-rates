using FluentValidation;
using ForeignExchangeRates.Core.Entities;
using ForeignExchangeRates.Core.Interfaces;
using System.Text.Json;

namespace ForeignExchangeRates.Core.Services;

public class ExchangeRateService : IExchangeRateService
{
	private readonly IExchangeRateRepository _exchangeRateRepository;
	private readonly IThirdPartyRatesProvider _thirdPartyRatesProvider;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IEventSourcingProvider _eventSourcingProvider;
	private readonly IValidator<ExchangeRate> _validator;

	public ExchangeRateService(IExchangeRateRepository exchangeRateRepository,
		IThirdPartyRatesProvider thirdPartyProvider, IUnitOfWork unitOfWork, 
		IEventSourcingProvider eventSourcingProvider, IValidator<ExchangeRate> validator)
	{
		_exchangeRateRepository = exchangeRateRepository;
		_thirdPartyRatesProvider = thirdPartyProvider;
		_unitOfWork = unitOfWork;
		_eventSourcingProvider = eventSourcingProvider;
		_validator = validator;
	}

	public async Task<ExchangeRate?> GetAsync(string sourceCurrencyCode, string targetCurrencyCode)
	{
		var exchangeRate = await _exchangeRateRepository.GetAsync(sourceCurrencyCode, targetCurrencyCode);
		if(exchangeRate == null)
		{
			exchangeRate = await _thirdPartyRatesProvider.GetAsync(sourceCurrencyCode, targetCurrencyCode);
			if(exchangeRate != null) 
			{
				await CreateAsync(exchangeRate);
			}
		}

		return exchangeRate;
	}

	public async Task<ServiceResult<ExchangeRate>> CreateAsync(ExchangeRate exchangeRate)
	{
		var existingExchangeRate = await _exchangeRateRepository.GetAsync(exchangeRate.SourceCurrencyCode, exchangeRate.TargetCurrencyCode);
		if(existingExchangeRate != null)
		{
			return ServiceResult<ExchangeRate>.Conflict();
		}

		var validationResult = await _validator.ValidateAsync(exchangeRate);
		if (!validationResult.IsValid)
		{
			return ServiceResult<ExchangeRate>.ValidationFailed(validationResult.ToDictionary());
		}

		await _exchangeRateRepository.InsertAsync(exchangeRate);

		await _unitOfWork.SaveChangesAsync();
		await _eventSourcingProvider.SaveEventAsync(new EventMessage()
		{
			EventType = EventType.Created,
			Body = JsonSerializer.Serialize(exchangeRate)
		});

		return ServiceResult<ExchangeRate>.Success(exchangeRate);
	}

	public async Task<ServiceResult<ExchangeRate>> UpdateAsync(ExchangeRate exchangeRate, bool validateExistence = true)
	{
		if(validateExistence)
		{
			var existingExchangeRate = await _exchangeRateRepository.GetAsync(exchangeRate.SourceCurrencyCode, exchangeRate.SourceCurrencyCode);
			if (existingExchangeRate == null)
			{
				return ServiceResult<ExchangeRate>.NotFound();
			}
		}

		var validationResult = await _validator.ValidateAsync(exchangeRate);
		if (!validationResult.IsValid)
		{
			return ServiceResult<ExchangeRate>.ValidationFailed(validationResult.ToDictionary());
		}

		_exchangeRateRepository.Update(exchangeRate);

		await _unitOfWork.SaveChangesAsync();

		return ServiceResult<ExchangeRate>.Success(exchangeRate);
	}

	public async Task<ServiceResult<ExchangeRate>> DeleteAsync(ExchangeRate exchangeRate, bool validateExistence = true)
	{
		if (validateExistence)
		{
			var existingExchangeRate = await _exchangeRateRepository.GetAsync(exchangeRate.SourceCurrencyCode, exchangeRate.SourceCurrencyCode);
			if (existingExchangeRate == null)
			{
				return ServiceResult<ExchangeRate>.NotFound();
			}
		}
		_exchangeRateRepository.Delete(exchangeRate);

		await _unitOfWork.SaveChangesAsync();

		return ServiceResult<ExchangeRate>.Success(exchangeRate);
	}
}
