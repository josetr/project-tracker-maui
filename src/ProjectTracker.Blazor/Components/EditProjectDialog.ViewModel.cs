namespace ProjectTracker.ViewModels;

using System.Threading.Tasks;
using ProjectTracker;

public sealed class EditProjectDialogViewModel : ViewModelBase
{
    private readonly Store _store;
    private string previousId = string.Empty;

    public EditProjectDialogViewModel(Store store)
    {
        _store = store;
    }

    [Parameter]
    public string Id { get; set; } = string.Empty;

    public async Task OpenAsync(string id)
    {
        if (!await _store.ProjectExistsAsync(id))
            return;
        previousId = id;
        Id = id;
    }

    public async Task<bool> EditAsync()
    {
        var task = await _store.GetProjectAsync(previousId);
        if (task == null)
            return false;

        task.Id = Id;
        await _store.UpdateProjectAsync(task, previousId);
        return true;
    }

    public async Task DeleteAsync()
    {
        await _store.RemoveProjectAsync(previousId);
    }
}
