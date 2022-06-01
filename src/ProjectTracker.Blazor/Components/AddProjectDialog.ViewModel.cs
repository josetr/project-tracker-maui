namespace ProjectTracker.ViewModels;

using System.Threading.Tasks;
using ProjectTracker;

public sealed class AddProjectDialogViewModel : ViewModelBase
{
    private readonly Store _store;

    public AddProjectDialogViewModel(Store store)
        : base()
    {
        _store = store;
    }

    public string Id { get; set; } = string.Empty;

    public async Task<Project?> AddAsync()
    {
        var project = new Project() { Id = Id };

        if (project == null || string.IsNullOrWhiteSpace(Id) || await _store.ProjectExistsAsync(Id))
            return null;

        await _store.AddProjectAsync(project);
        return project;
    }
}
