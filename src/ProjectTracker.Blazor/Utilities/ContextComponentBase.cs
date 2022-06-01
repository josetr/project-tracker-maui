namespace ProjectTracker;

using Microsoft.AspNetCore.Components;

public partial class Component<T> : ComponentBase, IAsyncDisposable, IDisposable where T : ViewModelBase
{
    [Inject] public T Model { get; set; } = default!;
    private readonly ParameterResolver resolver = new();

    protected override void OnInitialized()
    {
        Model.StateHasChanged = StateHasChanged;
        Model.InvokeAsync = InvokeAsync;

        base.OnInitialized();
        resolver.Resolve(this, Model);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        resolver.Resolve(this, Model);
    }

    protected override async Task OnInitializedAsync()
    {
        if (Model != null)
            await Model.OnInitializedAsync();
        await base.OnInitializedAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (Model != null && Model is IAsyncDisposable d)
            await d.DisposeAsync();
    }

    public void Dispose()
    {
        if (Model != null && Model is IDisposable d)
            d.Dispose();
    }
}
