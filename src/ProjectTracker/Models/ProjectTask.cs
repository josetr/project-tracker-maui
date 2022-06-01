namespace ProjectTracker;

using System;

public sealed class ProjectTask
{
    public int Id { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Project { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? CompleteDate { get; set; }
    public string? Repeat { get; set; }
    public bool RepeatEveryDay => true;
}
