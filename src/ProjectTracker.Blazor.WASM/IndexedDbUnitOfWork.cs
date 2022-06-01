namespace ProjectTracker.Blazor.WASM;

using TG.Blazor.IndexedDB;

public class IndexedDbUnitOfWork : IUnitOfWork
{
    private readonly IndexedDBManager dbManager;

    public IndexedDbUnitOfWork(IndexedDBManager dbManager)
    {
        Projects = new IndexedDbRepository<Project, string>(dbManager);
        Tasks = new IndexedDbRepository<ProjectTask, int>(dbManager);
        TaskTimeEntries = new IndexedDbRepository<TaskTimeEntry, int>(dbManager);
        this.dbManager = dbManager;
    }

    public IRepository<Project, string> Projects { get; }
    public IRepository<ProjectTask, int> Tasks { get; }
    public IRepository<TaskTimeEntry, int> TaskTimeEntries { get; }

    public Task EnsureCreatedAsync()
    {
        return dbManager.OpenDb();
    }
}
