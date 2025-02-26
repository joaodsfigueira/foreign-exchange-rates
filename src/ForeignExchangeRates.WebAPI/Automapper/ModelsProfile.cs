using AutoMapper;
using ForeignExchangeRates.Core.Entities;
using ForeignExchangeRates.WebAPI.Models;

namespace ForeignExchangeRates.WebAPI.Automapper;

public class ModelsProfile : Profile
{
	public ModelsProfile()
	{
		CreateMap<ExchangeRatePostModel, ExchangeRate>();
		CreateMap<ExchangeRatePutModel, ExchangeRate>();
		CreateMap<ExchangeRate, ExchangeRateDto>();
	}
}
