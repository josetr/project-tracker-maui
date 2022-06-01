namespace ProjectTracker.Blazor.WASM;

using TG.Blazor.IndexedDB;

public class IndexedDbRepository<T, TKey> : IRepository<T, TKey>, IDisposable where T : class
{
    private readonly IndexedDBManager dbManager;
    private readonly string storeName = typeof(T).Name;
    private string? _LastSuccessfulMessage { get; set; }
    public string? LastSuccessfulMessage => _LastSuccessfulMessage;

    public IndexedDbRepository(IndexedDBManager dbManager)
    {
        this.dbManager = dbManager;
        dbManager.ActionCompleted += DbManager_ActionCompleted;
    }

    public void Dispose()
    {
        dbManager.ActionCompleted -= DbManager_ActionCompleted;
    }

    public async Task AddAsync(T record)
    {
        _LastSuccessfulMessage = null;

        try
        {
            await dbManager.AddRecord(new StoreRecord<T>
            {
                Storename = storeName,
                Data = record
            });
        }
        finally
        {

        }
    }

    private void DbManager_ActionCompleted(object? sender, IndexedDBNotificationArgs e)
    {
        if (e.Outcome == IndexDBActionOutCome.Successful)
            _LastSuccessfulMessage = e.Message;
    }

    public async Task<T[]> GetAllAsync()
    {
        var records = await dbManager.GetRecords<T>(storeName);
        if (records == null)
            records = new List<T>();
        return records.ToArray();
    }

    public Task<T?> GetAsync(TKey id)
    {
        return dbManager.GetRecordById<TKey, T?>(storeName, id);
    }

    public Task UpdateAsync(T record)
    {
        return dbManager.UpdateRecord(new StoreRecord<T>
        {
            Storename = storeName,
            Data = record
        });
    }

    public Task RemoveAsync(TKey id)
    {
        return dbManager.DeleteRecord(storeName, id);
    }
}
