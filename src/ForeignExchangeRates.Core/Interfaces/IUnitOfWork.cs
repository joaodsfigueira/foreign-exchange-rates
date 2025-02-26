namespace ForeignExchangeRates.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
	Task SaveChangesAsync();
}
