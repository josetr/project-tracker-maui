namespace ProjectTracker.Models;

public class StoreState
{
    public List<Project> Projects { get; internal set; } = new();
    public List<ProjectTask> Tasks { get; internal set; } = new();
    public List<TaskTimeEntry> TimeEntries { get; internal set; } = new();
    public Project? SelectedProject = null;
    public string SelectedProjectId => SelectedProject?.Id ?? string.Empty;
}
