namespace ProjectTracker;

public interface IDialogService
{
    Task<TResult?> Show<TViewModel, TResult, TParameter>(string name, string parameterName, TParameter? parameter);
    Task<TResult?> Show<TViewModel, TResult>(string name);
    Task<bool?> ShowMessageBox(string title, string message, string yesText, string noText);
}
