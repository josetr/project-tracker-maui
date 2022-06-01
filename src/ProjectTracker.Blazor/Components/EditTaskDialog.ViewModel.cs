namespace ProjectTracker.ViewModels;

using System.Threading.Tasks;
using ProjectTracker;

public sealed class EditTaskDialogViewModel : ViewModelBase
{
    private readonly Store _store;

    public EditTaskDialogViewModel(Store store)
    {
        _store = store;
    }

    [Parameter]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProjectId { get; set; } = string.Empty;
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int Seconds { get; set; }
    public TimeSpan Duration => new(Hours, Minutes, Seconds);

    public async Task OpenAsync(int id)
    {
        var task = await _store.GetTaskAsync(id);
        if (task == null)
            return;
        this.Id = id;
        Name = task.Name;
        ProjectId = task.Project;
        Hours = task.Duration.Hours;
        Minutes = task.Duration.Minutes;
        Seconds = task.Duration.Seconds;
    }

    public async Task<bool> EditAsync()
    {
        var task = await _store.GetTaskAsync(Id);
        if (task == null)
            return false;

        task.Name = Name;
        task.Duration = Duration;
        await _store.UpdateTaskAsync(task);
        return true;
    }

    public async Task DeleteAsync()
    {
        await _store.RemoveTaskAsync(Id);
    }
}
