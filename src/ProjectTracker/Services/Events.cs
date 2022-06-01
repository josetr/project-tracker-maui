namespace ProjectTracker;

public delegate void TaskEventDelegate(object evt);
public record ProjectAdded(Project project);
public record OpenAddTaskDialogEvent();
public record OpenAddProjectDialogEvent();
public record ToggleStartTaskEvent();
public record ProjectDeleted(string id);
public record SelectedProjectChanged(Project? project);
public record TaskTimeEntryDeleted(int taskId);
public record TaskAdded(ProjectTask task);
public record TaskEdited(ProjectTask task);
public record TaskDeleted(ProjectTask task);
public record TaskCompleted(ProjectTask task);

public sealed class TaskEvents
{
    public event TaskEventDelegate? EventAdded;
    public void Raise(object evt) => EventAdded?.Invoke(evt);
}
