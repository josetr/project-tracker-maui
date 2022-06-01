namespace ProjectTracker.ViewModels;
using System.Threading.Tasks;
using ProjectTracker;
using ProjectTracker.Models;

public sealed class LayoutViewModel : ViewModelBase, IDisposable
{
    private readonly ProjectService _projectService;
    private readonly TaskEvents _taskEvents;
    private readonly Store _store;
    private readonly IDialogService _dialogService;
    private readonly IHotkeyManager _hotkeyManager;

    public LayoutViewModel(
        ProjectService projectService,
        TaskEvents taskEvents,
        Store store,
        IDialogService dialogService,
        IHotkeyManager hotkeyManager)
    {
        _projectService = projectService;
        _taskEvents = taskEvents;
        _store = store;
        _dialogService = dialogService;
        _hotkeyManager = hotkeyManager;
    }

    public List<Project> Projects { get; private set; } = new();
    public bool IsInitialized { get; set; } = false;
    public bool IsDrawerOpen { get; set; } = true;
    public string SelectedProjectId => _store.State.SelectedProjectId;

    public override async Task OnInitializedAsync()
    {
        if (IsInitialized)
            return;

        await _projectService.InitializeAsync();
        Projects = await _store.GetProjects();

        _taskEvents.EventAdded += OnEvent;
        _hotkeyManager.RegisterHandler((hotkey) =>
        {
            if (hotkey.Modifiers.HasFlag(KeyModifiers.Menu) &&
                hotkey.Modifiers.HasFlag(KeyModifiers.Control))
            {
                if (hotkey.Key == Key.T)
                    InvokeAsync(() => _taskEvents.Raise(new OpenAddTaskDialogEvent()));
                else if (hotkey.Key == Key.S)
                    InvokeAsync(() => _taskEvents.Raise(new ToggleStartTaskEvent()));
            }
        });
        _hotkeyManager.RegisterHotkey(100, new Hotkey()
        {
            Modifiers = KeyModifiers.Control | KeyModifiers.Menu,
            Key = Key.T
        });
        _hotkeyManager.RegisterHotkey(200, new Hotkey()
        {
            Modifiers = KeyModifiers.Control | KeyModifiers.Menu,
            Key = Key.S
        });
        IsInitialized = true;
    }

    public void Dispose()
    {
        _taskEvents.EventAdded -= OnEvent;
        _hotkeyManager.UnregisterHotkey(100);
        _hotkeyManager.UnregisterHotkey(200);
    }

    public async Task OpenCreateProjectDialog()
    {
        await _dialogService.Show<AddProjectDialogViewModel, bool>("Create project");
    }

    public async Task OpenEditProjectDialog(string id)
    {
        await _dialogService.Show<EditProjectDialogViewModel, bool, string>("Edit project", nameof(EditProjectDialogViewModel.Id), id);
    }

    public async Task OpenRemoveProjectDialog(string id)
    {
        if (await _dialogService.ShowMessageBox("Remove task", $"Do you really want to delete the '{id}' project?", "yes", noText: "no") == true)
            await _store.RemoveProjectAsync(id);
    }

    public void SelectProject(int n)
    {
        var project = Projects.GetNextItem(Projects.FindIndex(m => m.Id == _store.State.SelectedProjectId), n);
        if (!Projects.Any())
            return;

        if (project != null)
            _store.SelectProject(project);
    }

    public void SelectProject(Project project)
    {
        _store.SelectProject(project);
    }

    private void OnEvent(object obj)
    {
    }

    public void ToggleDrawer()
    {
        IsDrawerOpen = !IsDrawerOpen;
    }
}
