namespace ProjectTracker;

public class ViewModelBase
{
    public Action StateHasChanged = () => { };
    public Func<Action, Task> InvokeAsync = action => Task.CompletedTask;

    public virtual Task OnInitializedAsync()
    {
        return Task.CompletedTask;
    }
}
