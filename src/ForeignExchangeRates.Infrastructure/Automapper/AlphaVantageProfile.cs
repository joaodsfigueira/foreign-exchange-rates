using AutoMapper;
using ForeignExchangeRates.Core.Entities;
using ForeignExchangeRates.Infrastructure.Providers;

namespace ForeignExchangeRates.Infrastructure.Automapper;

public class AlphaVantageProfile : Profile
{
	public AlphaVantageProfile()
	{
		CreateMap<AlphaVantageRealtimeCurrencyExchangeRate, ExchangeRate>()
			.ForMember(dest => dest.ExchangeRateValue, a => a.MapFrom(src => decimal.Parse(src.ExchangeRateValue)))
			.ForMember(dest => dest.BidPrice, a => a.MapFrom(src => decimal.Parse(src.BidPrice)))
			.ForMember(dest => dest.AskPrice, a => a.MapFrom(src => decimal.Parse(src.AskPrice)));
	}
}
