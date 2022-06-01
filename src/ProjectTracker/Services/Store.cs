namespace ProjectTracker;

using System.Threading.Tasks;
using ProjectTracker.Models;

public class Store
{
    private readonly ProjectService projectService;
    private readonly TaskService taskService;
    private readonly StoreState state;
    private readonly TaskEvents events;

    public Store(ProjectService projectService, TaskService taskService, StoreState storeState, TaskEvents events)
    {
        this.projectService = projectService;
        this.taskService = taskService;
        this.state = storeState;
        this.events = events;
    }

    public StoreState State => state;

    public async Task<List<Project>> GetProjects()
    {
        if (state.Projects.Count == 0)
        {
            state.Projects = (await projectService.GetProjectsAsync()).ToList();
            state.Projects.OrderBy(x => x.Id);
            state.SelectedProject = state.Projects.FirstOrDefault();
        }

        return state.Projects;
    }

    public Task<bool> ProjectExistsAsync(string id)
    {
        return Task.FromResult(state.Projects.Any(x => x.Id == id));
    }

    public Task<bool> TaskExistsAsync(string projectId, string taskName)
    {
        return Task.FromResult(state.Tasks.Any(x => x.Project == projectId && x.Name == taskName));
    }

    public Task<Project?> GetProjectAsync(string id)
    {
        return Task.FromResult(state.Projects.FirstOrDefault(x => x.Id == id));
    }

    public Task<ProjectTask?> GetTaskAsync(int id)
    {
        return Task.FromResult(state.Tasks.FirstOrDefault(x => x.Id == id));
    }

    public async Task<List<ProjectTask>> GetTasks()
    {
        if (state.Tasks.Count == 0)
            state.Tasks = (await taskService.GetTasksAsync()).ToList();

        return state.Tasks;
    }

    public async Task AddProjectAsync(Project project)
    {
        await projectService.AddProjectAsync(project);
        state.Projects.Add(project);
        state.Projects.OrderBy(x => x.Id);
        SelectProject(project);
        events.Raise(new ProjectAdded(project));
    }

    public async Task RemoveProjectAsync(string id)
    {
        foreach (var task in (await GetTasks()).ToArray())
        {
            if (task.Project == id)
                await RemoveTaskAsync(task.Id);
        }

        await projectService.RemoveProjectAsync(id);
        state.Projects.RemoveAll(x => x.Id == id);

        if (state.SelectedProject != null && state.SelectedProject.Id == id)
            SelectProject(state.Projects.FirstOrDefault());

        events.Raise(new ProjectDeleted(id));
    }

    public async Task UpdateTaskAsync(ProjectTask task)
    {
        await taskService.UpdateTaskAsync(task);
        events.Raise(new TaskEdited(task));
    }

    public async Task UpdateProjectAsync(Project project, string previousId)
    {
        await projectService.UpdateProjectAsync(project, previousId);
        if (project.Id == previousId)
            return;

        foreach (var task in await GetTasks())
        {
            if (task.Project == previousId)
            {
                task.Project = project.Id;
                await UpdateTaskAsync(task);
            }
        }
    }

    public async Task AddTaskAsync(ProjectTask task)
    {
        await taskService.AddTaskAsync(task);
        state.Tasks.Add(task);
        events.Raise(new TaskAdded(task));
    }

    public async Task RemoveTaskAsync(int id)
    {
        if (!await taskService.RemoveTaskAsync(id))
            return;
        var task = state.Tasks.First(x => x.Id == id);
        state.Tasks.Remove(task);
        events.Raise(new TaskDeleted(task));
    }

    public async Task<List<TaskTimeEntry>> GetTaskHistoryAsync()
    {
        if (state.TimeEntries.Count == 0)
            state.TimeEntries = (await taskService.GetTaskHistoryAsync()).ToList();

        return state.TimeEntries;
    }

    public async Task AddTaskTimeEntryAsync(TaskTimeEntry timeEntry)
    {
        await taskService.AddTaskTimeEntryAsync(timeEntry);
        state.TimeEntries.Add(timeEntry);
    }

    public async Task<TimeSpan> CalculateTotalTaskElapsedTime(int id, TimePeriod period)
    {
        var entries = await GetTaskHistoryAsync();
        var fentries =
            period == TimePeriod.Year
            ? entries.Where(x =>
                x.StartDate.Year == DateTime.UtcNow.Year &&
                x.TaskId == id)
            :
            period == TimePeriod.Month
            ? entries.Where(x =>
                x.StartDate.Year == DateTime.UtcNow.Year &&
                x.StartDate.Month == DateTime.UtcNow.Month &&
                x.TaskId == id)
            :
            period == TimePeriod.Day
            ? entries.Where(x =>
                x.StartDate.Year == DateTime.UtcNow.Year &&
                x.StartDate.Month == DateTime.UtcNow.Month &&
                x.StartDate.Day == DateTime.UtcNow.Day &&
                x.TaskId == id)
            : entries.Where(x => x.TaskId == id);

        return TimeSpan.FromSeconds(fentries.Sum(x => x.ElapsedSeconds));
    }

    public void SelectProject(Project? project)
    {
        state.SelectedProject = project;
        events.Raise(new SelectedProjectChanged(project));
    }

    public async Task DeleteTaskHistoryEntry(int id)
    {
        await taskService.DeleteHistoryEntryAsync(id);
        var entry = state.TimeEntries.FirstOrDefault(x => x.Id == id);
        if (entry == null)
            return;
        state.TimeEntries.Remove(entry);
        events.Raise(new TaskTimeEntryDeleted(entry.TaskId));
    }
}
