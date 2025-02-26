namespace ForeignExchangeRates.Core.Interfaces;

public interface IGenericRepository<T> where T : class
{
	IEnumerable<T> Get(int skip, int take);
	Task InsertAsync(T entity);
	void Update(T entity);
	void Delete(T entity);
}
