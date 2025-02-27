namespace ForeignExchangeRates.Core.Interfaces;

public interface IGenericRepository<T> where T : class
{
	Task InsertAsync(T entity);
	void Update(T entity);
	void Delete(T entity);
}
