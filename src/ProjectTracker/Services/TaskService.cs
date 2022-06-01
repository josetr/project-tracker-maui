namespace ProjectTracker;

using System;
using System.Linq;
using System.Threading.Tasks;

public sealed class TaskService
{
    private readonly IUnitOfWork dbContext;
    private readonly TaskEvents events;

    public TaskService(IUnitOfWork dbContext, TaskEvents events)
    {
        this.dbContext = dbContext;
        this.events = events;
    }

    public Task InitializeAsync()
    {
        return dbContext.EnsureCreatedAsync();
    }

    public async Task<ProjectTask[]> GetTasksAsync()
    {
        return (await dbContext.Tasks.GetAllAsync()).ToArray();
    }

    public async Task AddTaskAsync(ProjectTask task)
    {
        await dbContext.Tasks.AddAsync(task);

        if (dbContext.Tasks.LastSuccessfulMessage != null)
            task.Id = int.Parse(dbContext.Tasks.LastSuccessfulMessage.Split(' ').Last());
    }

    public async Task<bool> RemoveTaskAsync(int id)
    {
        var task = await GetTaskAsync(id);
        if (task == null)
            return false;

        await dbContext.Tasks.RemoveAsync(task.Id);
        return true;
    }

    public async Task UpdateTaskAsync(ProjectTask task)
    {
        await dbContext.Tasks.UpdateAsync(task);
    }

    public Task AddTaskTimeEntryAsync(TaskTimeEntry timeEntry)
    {
        return dbContext.TaskTimeEntries.AddAsync(timeEntry);
    }

    public async Task<TaskTimeEntry[]> GetTaskHistoryAsync()
    {
        return (await dbContext.TaskTimeEntries.GetAllAsync()).ToArray();
    }

    public async Task DeleteHistoryEntryAsync(int id)
    {
        var task = await dbContext.TaskTimeEntries.GetAsync(id);
        if (task == null)
            return;

        await dbContext.TaskTimeEntries.RemoveAsync(task.Id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await dbContext.Tasks.GetAsync(id) != null;
    }

    public async Task<bool> CompleteTaskAsync(int id, DateTime time)
    {
        var task = await GetTaskAsync(id);
        if (task == null)
            return false;

        task.CompleteDate = time;
        await dbContext.Tasks.UpdateAsync(task);
        events.Raise(new TaskCompleted(task));
        return true;
    }

    public async Task<bool> UndoTaskAsync(int id)
    {
        var task = await GetTaskAsync(id);
        if (task == null || task.CompleteDate == null)
            return false;

        task.CompleteDate = null;
        await dbContext.Tasks.UpdateAsync(task);
        return true;
    }

    public Task<ProjectTask?> GetTaskAsync(int id)
    {
        return dbContext.Tasks.GetAsync(id);
    }
}
