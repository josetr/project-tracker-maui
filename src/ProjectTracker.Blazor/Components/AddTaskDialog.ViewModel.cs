namespace ProjectTracker.ViewModels;

using System.Threading.Tasks;
using ProjectTracker;

public sealed class AddTaskDialogViewModel : ViewModelBase
{
    private readonly Store _store;
    private readonly ProjectTask _task = new();

    public AddTaskDialogViewModel(Store store)
        : base()
    {
        _store = store;
        ProjectId = _store.State.SelectedProjectId;
    }

    public string Name { get; set; } = string.Empty;
    public string ProjectId { get; set; } = string.Empty;
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int Seconds { get; set; }
    public TimeSpan Duration => new(Hours, Minutes, Seconds);

    public async Task<ProjectTask?> AddAsync()
    {
        var task = _task;
        if (task == null || string.IsNullOrWhiteSpace(Name))
            return null;

        task.Name = Name;
        task.Duration = Duration;
        task.Project = ProjectId;
        await _store.AddTaskAsync(task);
        return task;
    }
}
