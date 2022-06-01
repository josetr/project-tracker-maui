namespace ProjectTracker;

public interface IUnitOfWork
{
    IRepository<Project, string> Projects { get; }
    IRepository<ProjectTask, int> Tasks { get; }
    IRepository<TaskTimeEntry, int> TaskTimeEntries { get; }
    Task EnsureCreatedAsync();
}
