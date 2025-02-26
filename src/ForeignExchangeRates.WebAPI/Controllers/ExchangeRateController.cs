using AutoMapper;
using ForeignExchangeRates.Core.Entities;
using ForeignExchangeRates.Core.Interfaces;
using ForeignExchangeRates.Core.Services;
using ForeignExchangeRates.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ForeignExchangeRates.WebAPI.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/exchange-rates")]
public class ExchangeRateController : ControllerBase
{
	private readonly IExchangeRateRepository _exchangeRateRepository;
	private readonly IExchangeRateService _exchangeRateService;
	private readonly IMapper _mapper;

    public ExchangeRateController(IExchangeRateRepository exchangeRateRepository, IExchangeRateService exchangeRateService,
		IMapper mapper)
    {
		_exchangeRateRepository = exchangeRateRepository;
		_exchangeRateService = exchangeRateService;
		_mapper = mapper;
    }

	[HttpGet("{sourceCurrencyCode}/{targetCurrencyCode}")]
	[ProducesResponseType(typeof(ExchangeRateDto), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.NotFound)]
	public async Task<IActionResult> GetAsync(string sourceCurrencyCode, string targetCurrencyCode)
	{
		var exchangeRate = await _exchangeRateService.GetAsync(sourceCurrencyCode, targetCurrencyCode);
		if(exchangeRate == null)
		{
			return NotFound();
		}
		return Ok(_mapper.Map<ExchangeRateDto>(exchangeRate));
	}

	[HttpPost("{sourceCurrencyCode}/{targetCurrencyCode}")]
	[ProducesResponseType(typeof(ExchangeRateDto), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(IDictionary<string, IEnumerable<string>>), (int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.Conflict)]
	public async Task<IActionResult> InsertAsync(string sourceCurrencyCode, string targetCurrencyCode, [FromBody] ExchangeRatePostModel postModel)
	{
		var exchangeRate = _mapper.Map<ExchangeRate>(postModel, opt => {
			opt.AfterMap((src, dest) => {
				dest.SourceCurrencyCode = sourceCurrencyCode;
				dest.TargetCurrencyCode = targetCurrencyCode;
			});
		});
		var serviceResult = await _exchangeRateService.CreateAsync(exchangeRate);
		switch (serviceResult)
		{
			case SuccessResult<ExchangeRate> successResult:
				return Ok(_mapper.Map<ExchangeRateDto>(successResult.ResultObject));
			case ValidationFailedResult<ExchangeRate> validationFailedResult:
				return BadRequest(validationFailedResult.Errors);
			case ConflictResult<ExchangeRate>:
				return Conflict();
			default:
				throw new InvalidOperationException("Service result not expected");
		}
	}

	[HttpPut("{sourceCurrencyCode}/{targetCurrencyCode}")]
	[ProducesResponseType(typeof(ExchangeRateDto), (int)HttpStatusCode.OK)]
	[ProducesResponseType((int)HttpStatusCode.NotFound)]
	[ProducesResponseType(typeof(IDictionary<string, IEnumerable<string>>), (int)HttpStatusCode.BadRequest)]
	public async Task<IActionResult> UpdateAsync(string sourceCurrencyCode, string targetCurrencyCode,
		[FromBody] ExchangeRatePutModel putModel)
	{
		var existingExchangeRate = await _exchangeRateRepository.GetAsync(sourceCurrencyCode, targetCurrencyCode);
		if(existingExchangeRate == null)
		{
			return NotFound();
		}

		var exchangeRate = _mapper.Map(putModel, existingExchangeRate);
		var serviceResult = await _exchangeRateService.UpdateAsync(exchangeRate, false);
		switch (serviceResult)
		{
			case SuccessResult<ExchangeRate> successResult:
				return Ok(_mapper.Map<ExchangeRateDto>(successResult.ResultObject));
			case ValidationFailedResult<ExchangeRate> validationFailedResult:
				return BadRequest(validationFailedResult.Errors);
			case NotFoundResult<ExchangeRate>:
				return NotFound();
			default:
				throw new InvalidOperationException("Service result not expected");
		}
	}

	[HttpDelete("{sourceCurrencyCode}/{targetCurrencyCode}")]
	[ProducesResponseType((int)HttpStatusCode.NoContent)]
	[ProducesResponseType((int)HttpStatusCode.NotFound)]
	public async Task<IActionResult> DeleteAsync(string sourceCurrencyCode, string targetCurrencyCode)
	{
		var exchangeRate = await _exchangeRateRepository.GetAsync(sourceCurrencyCode, targetCurrencyCode);
		if (exchangeRate == null)
		{
			return NotFound();
		}

		var serviceResult = await _exchangeRateService.DeleteAsync(exchangeRate, false);
		switch (serviceResult)
		{
			case SuccessResult<ExchangeRate> successResult:
				return NoContent();
			case NotFoundResult<ExchangeRate>:
				return NotFound();
			default:
				throw new InvalidOperationException("Service result not expected");
		}
	}
}
