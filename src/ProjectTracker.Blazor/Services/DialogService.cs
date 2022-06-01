namespace ProjectTracker;

using System.Diagnostics.CodeAnalysis;

public class DialogService : IDialogService
{
    private readonly static Dictionary<Type, Type> Dialogs = new();
    private readonly MudBlazor.IDialogService _mudDialogService;

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, "MudBlazor.MudMessageBox", "MudBlazor")]
    public DialogService(MudBlazor.IDialogService mudDialogService)
    {
        _mudDialogService = mudDialogService;
    }

    public async Task<TResult?> Show<TViewModel, TResult, TParameter>(string name, string parameterName, TParameter? parameter = default)
    {
        if (!Dialogs.TryGetValue(typeof(TViewModel), out var componentType))
            return default;

        var parameters = new MudBlazor.DialogParameters() { { parameterName, parameter } };
        var dialogReference = _mudDialogService.Show(componentType, name, parameters, GetDialogOptions());
        return (await dialogReference.Result).Data is TResult result ? result : default;
    }

    public async Task<TResult?> Show<TViewModel, TResult>(string name)
    {
        if (!Dialogs.TryGetValue(typeof(TViewModel), out var componentType))
            return default;

        var dialogReference = _mudDialogService.Show(componentType, name, options: GetDialogOptions());
        return (await dialogReference.Result).Data is TResult result ? result : default;
    }

    public Task<bool?> ShowMessageBox(string title, string message, string yesText, string noText)
    {
        return _mudDialogService.ShowMessageBox(title, message, yesText, noText, options: GetDialogOptions());
    }

    private static MudBlazor.DialogOptions GetDialogOptions()
    {
        return new MudBlazor.DialogOptions() { CloseButton = true, CloseOnEscapeKey = true };
    }

    public static void Register<TViewModel, TDialog>()
    {
        Dialogs.Add(typeof(TViewModel), typeof(TDialog));
    }
}
