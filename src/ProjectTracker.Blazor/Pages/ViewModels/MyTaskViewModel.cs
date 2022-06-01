namespace ProjectTracker.ViewModels;

using System;

public sealed class ProjectTaskViewModel
{
    public int Id { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string ProjectId { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? CompleteDate { get; set; }
    public string? Repeat { get; set; }
    public bool RepeatEveryDay => true;
    public TimeSpan TickElapsedTime { get; set; }
    public TimeSpan PeriodElapsedTime { get; set; }
    public TimeSpan ElapsedTime { get; set; }
    public bool IsExpanded { get; set; }
    public List<TaskTimeEntry> History { get; set; } = new();
    public bool IsCompleted => CompleteDate != null;
    public double Progress => ElapsedTime.TotalSeconds / Duration.TotalSeconds;
    public bool TrackTime => true;
    public bool IsCompleteDateAvailable => CompleteDate != null;
    public bool IsActuallyCompleted => IsCompleteDateAvailable && (!RepeatEveryDay || (CompleteDate != null && DateTime.UtcNow.Day == CompleteDate.Value.Day));
    public TimeSpan RemainingTime
    {
        get
        {
            return ElapsedTime > Duration ? ElapsedTime - Duration : Duration - ElapsedTime;
        }
    }
    public string PrettyRemainingTime => TimeUtil.FormatElapsedTime(RemainingTime);

    internal TimeSpan TotalElapsedTime;
    internal DateTime runStartTime;
    internal DateTime runBreakStartTime;
    internal DateTime runBreakEndTime;

    public TaskTimeEntry CompleteTimeEntry(DateTime now)
    {
        var elapsed = GetRunElapsedTime(now);
        TotalElapsedTime += elapsed;

        var timeEntry = new TaskTimeEntry()
        {
            Name = Name,
            TaskId = Id,
            StartDate = runStartTime,
            StopDate = now,
            ElapsedSeconds = elapsed.TotalSeconds
        };

        runStartTime = default;
        runBreakStartTime = default;
        runBreakEndTime = default;
        return timeEntry;
    }

    public TimeSpan GetRunElapsedTime(DateTime now) => now - runStartTime;
    public TimeSpan RunElapsedTime => DateTime.UtcNow - runStartTime;

    public void StartBreak(DateTime now)
    {
        var elapsed = now - runBreakStartTime;
        runBreakEndTime = now + TimeSpan.FromSeconds(elapsed.TotalSeconds / 5);
    }
}
