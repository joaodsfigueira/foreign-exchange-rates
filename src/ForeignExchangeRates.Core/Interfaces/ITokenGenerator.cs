using ForeignExchangeRates.Core.Entities;

namespace ForeignExchangeRates.Core.Interfaces;

public interface ITokenGenerator
{
    string CreateToken(User user);
}
