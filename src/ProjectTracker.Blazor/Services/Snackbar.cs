namespace ProjectTracker;

public class Snackbar : ISnackbar
{
    public Snackbar(MudBlazor.ISnackbar mudSnackbar)
    {
        MudSnackbar = mudSnackbar;
    }

    public MudBlazor.ISnackbar MudSnackbar { get; set; } = default!;

    public void Show(string message, Severity severity)
    {
        MudSnackbar.Clear();
        MudSnackbar.Configuration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomCenter;
        MudSnackbar.Configuration.VisibleStateDuration = 1500;

        MudSnackbar.Add(message, severity switch
        {
            Severity.Warning => MudBlazor.Severity.Warning,
            Severity.Success => MudBlazor.Severity.Success,
            Severity.Error => MudBlazor.Severity.Error,
            _ => throw new NotImplementedException()
        });
    }
}
