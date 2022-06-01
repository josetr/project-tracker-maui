namespace ProjectTracker;

using ProjectTracker.Blazor.WASM;

public class EFUnitOfWork : IUnitOfWork
{
    public EFUnitOfWork(TaskTrackerDbContext context)
    {
        Projects = new EFRepository<Project, string>(context);
        Tasks = new EFRepository<ProjectTask, int>(context);
        TaskTimeEntries = new EFRepository<TaskTimeEntry, int>(context);
        Context = context;
    }

    public IRepository<Project, string> Projects { get; }
    public IRepository<ProjectTask, int> Tasks { get; }
    public IRepository<TaskTimeEntry, int> TaskTimeEntries { get; }
    public TaskTrackerDbContext Context { get; }

    public Task EnsureCreatedAsync()
    {
        Context.Database.EnsureCreated();
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync()
    {
        return Context.SaveChangesAsync();
    }
}
