namespace ProjectTracker;

public interface IRepository<T, TKey> where T : class
{
    string? LastSuccessfulMessage => null;
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task RemoveAsync(TKey id);
    Task<T[]> GetAllAsync();
    Task<T?> GetAsync(TKey id);
}
