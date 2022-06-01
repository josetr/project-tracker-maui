namespace ProjectTracker;

using System;

public sealed class TaskTimeEntry
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime StopDate { get; set; }
    public double ElapsedSeconds { get; set; }
}
