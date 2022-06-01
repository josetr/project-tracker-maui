namespace ProjectTracker.ViewModels;

using System.Threading.Tasks;
using ProjectTracker;

public sealed class HistoryPageViewModel : ViewModelBase
{
    private readonly Store _store;
    private readonly INavigation navigation;

    public HistoryPageViewModel(Store store, INavigation navigation)
        : base()
    {
        this._store = store;
        this.navigation = navigation;
    }

    public List<TaskTimeEntry> TimeEntries { get; set; } = new();
    public string SearchText { get; set; } = string.Empty;

    public override async Task OnInitializedAsync()
    {
        TimeEntries = await _store.GetTaskHistoryAsync();
    }

    public void NavigateToHomeComponent()
    {
        navigation.NavigateTo("");
    }

    public async Task DeleteEntryAsync(int id)
    {
        await _store.DeleteTaskHistoryEntry(id);
    }
}
