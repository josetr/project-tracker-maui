namespace ProjectTracker.Blazor.WASM;

using Microsoft.EntityFrameworkCore;

public class EFRepository<T, TKey> : IRepository<T, TKey> where T : class
{
    private readonly TaskTrackerDbContext dbContext;
    private readonly DbSet<T> set;

    public EFRepository(TaskTrackerDbContext dbContext)
    {
        set = dbContext.Set<T>();
        this.dbContext = dbContext;
    }

    public async Task AddAsync(T record)
    {
        await set.AddAsync(record);
        await dbContext.SaveChangesAsync();
    }

    public Task<T[]> GetAllAsync()
    {
        return Task.FromResult(set.ToArray());
    }

    public async Task<T?> GetAsync(TKey id)
    {
        return await set.FindAsync(id);
    }

    public async Task RemoveAsync(TKey id)
    {
        var item = await GetAsync(id);
        if (item != null)
            set.Remove(item);

        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(T record)
    {
        await dbContext.SaveChangesAsync();
    }
}
