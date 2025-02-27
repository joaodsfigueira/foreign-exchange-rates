using AutoMapper;
using ForeignExchangeRates.Core.Entities;
using ForeignExchangeRates.Core.Services;
using ForeignExchangeRates.WebAPI.Models;

namespace ForeignExchangeRates.WebAPI.Automapper;

public class ModelsProfile : Profile
{
	public ModelsProfile()
	{
		CreateMap<UserPostModel, User>();
		CreateMap<User, UserDto>();
		CreateMap<TokenResponse, TokenResponseDto>();
		CreateMap<ExchangeRatePostModel, ExchangeRate>();
		CreateMap<ExchangeRatePutModel, ExchangeRate>();
		CreateMap<ExchangeRate, ExchangeRateDto>();
	}
}
