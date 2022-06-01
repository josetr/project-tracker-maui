namespace ProjectTracker;

public enum Severity
{
    Warning,
    Success,
    Error,
}

public interface ISnackbar
{
    void Show(string msg, Severity severity);
}
