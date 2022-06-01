namespace ProjectTracker.ViewModels;

using Microsoft.AspNetCore.Components.Web;
using System;
using System.Linq;
using System.Threading.Tasks;
using ProjectTracker;
using ProjectTracker.Models;

public sealed class IndexPageViewModel : ViewModelBase, IAsyncDisposable
{
    private const string Title = "Project Tracker";
    private readonly INotificationManager _notificationManager;
    private readonly TaskService _taskService;
    private readonly TaskEvents _taskEvents;
    private readonly IDialogService _dialogService;
    private readonly INavigation _navigation;
    private readonly Store _store;
    private State _state = State.Stopped;
    private bool _isTimerRunning = false;
    private Task _taskTimer = Task.CompletedTask;

    public IndexPageViewModel(
        INotificationManager notificationManager,
        TaskService taskService,
        TaskEvents taskEvents,
        Store store,
        IDialogService dialogService,
        INavigation navigation)
    {
        _notificationManager = notificationManager;
        _taskService = taskService;
        _taskEvents = taskEvents;
        _dialogService = dialogService;
        _navigation = navigation;
        _store = store;
    }

    private enum State
    {
        Stopped,
        Running,
        Break,
    }

    public ProjectTaskViewModel? RunningTask { get; private set; }
    public List<ProjectTaskViewModel> Tasks { get; private set; } = new();
    public IEnumerable<ProjectTaskViewModel> FilteredTasks = Enumerable.Empty<ProjectTaskViewModel>();
    public IEnumerable<ProjectTaskViewModel> FilteredTasks2 = Enumerable.Empty<ProjectTaskViewModel>();
    public TimeSpan MaxBreakTime { get; private set; }
    public TimePeriod TimePeriod { get; set; } = TimePeriod.Day;
    public bool IsSearchOpen { get; private set; }
    public bool Initialized { get; set; }
    public string ProjectId => _store.State.SelectedProjectId;
    public ProjectTaskViewModel? SelectedTask { get; set; }
    public int SelectedTaskId => SelectedTask?.Id ?? 0;
    public string SelectedTaskName => SelectedTask?.Name ?? "";
    public string SearchText { get; set; } = string.Empty;
    private bool IsTaskRunning => _state != State.Stopped;

    public override async Task OnInitializedAsync()
    {
        await _taskService.InitializeAsync();
        var projects = await _store.GetProjects();

        await LoadTasks();

        FilteredTasks = Tasks;
        _taskEvents.EventAdded += TaskEventAdded;
        Initialized = true;
    }

    public async Task LoadTasks()
    {
        if (string.IsNullOrWhiteSpace(ProjectId))
            return;

        Tasks.Clear();

        foreach (var task in await _store.GetTasks())
        {
            if (task.Project != ProjectId)
                continue;

            var elapsedTime = await _store.CalculateTotalTaskElapsedTime(task.Id, TimePeriod);
            var taskVm = CreateTaskVm(task);
            taskVm.ElapsedTime = elapsedTime;
            Tasks.Add(taskVm);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _taskEvents.EventAdded -= TaskEventAdded;
        await StopAsync();
    }

    public async Task RemoveTaskAsync(int id)
    {
        await _store.RemoveTaskAsync(id);
    }

    public bool Filter(ProjectTaskViewModel task)
    {
        return task.Name.ToLower().Contains(SearchText.ToLower());
    }

    public async Task OpenCreateTaskDialog()
    {
        await _dialogService.Show<AddTaskDialogViewModel, bool>("Create task");
    }

    public void SelectTask(ProjectTaskViewModel task)
    {
        if (SelectedTask == task)
            SelectedTask = null;
        else
            SelectedTask = task;
    }

    public void SelectNextTask(int n)
    {
        var task = Tasks.GetNextItem(Tasks.FindIndex(m => m.Id == SelectedTaskId), n);
        if (!Tasks.Any())
            return;

        if (task != null)
            SelectedTask = task;
    }

    public async Task Expand(ProjectTaskViewModel task)
    {
        task.IsExpanded = !task.IsExpanded;
        if (!task.IsExpanded)
            return;

        var result = (await _store.GetTaskHistoryAsync()).OrderByDescending(x => x.StartDate).AsEnumerable();
        task.History = result.Where(x => x.TaskId == task.Id).ToList();
    }

    public void NavigateToHistoryComponent()
    {
        _navigation.NavigateTo("history");
    }

    public async Task OpenTaskEditDialog(int id)
    {
        await _dialogService.Show<EditTaskDialogViewModel, bool, int>("Edit task", nameof(EditTaskDialogViewModel.Id), id);
    }

    public async Task OpenDeleteTaskConfirmDialog(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;

        if (await _dialogService.ShowMessageBox("Delete task", $"Do you really want to delete the '{name}' task?", "yes", noText: "no") == true)
            await RemoveTaskAsync(id);
    }

    public void OpenSearchView()
    {
        SearchText = string.Empty;
        IsSearchOpen = !IsSearchOpen;
    }

    public async Task CompleteTaskAsync(int id, bool complete)
    {
        if (complete)
            await CompleteTaskAsync(id);
        else
            await UndoCompleteTaskAsync(id);
    }

    public async Task CompleteTaskAsync(int id)
    {
        var task = GetTaskById(id);
        if (task != null)
            await CompleteTaskAsync(task, manual: true);

        FilteredTasks = FilteredTasks.OrderBy(x => x.IsCompleted);
    }

    public async Task ToggleCompleteStatus(int id)
    {
        var task = GetTaskById(id);
        if (task == null)
            return;

        if (!task.IsCompleted)
            await CompleteTaskAsync(task.Id);
        else
            await UndoCompleteTaskAsync(task.Id);

        StateHasChanged();
    }

    public async Task UndoCompleteTaskAsync(int id)
    {
        var task = Tasks.FirstOrDefault(x => x.Id == id);
        if (task != null && await _taskService.UndoTaskAsync(task.Id))
            task.CompleteDate = null;

        FilteredTasks = FilteredTasks.OrderBy(x => x.IsCompleted);
    }

    public ProjectTaskViewModel? GetTaskByName(string name)
    {
        return Tasks.FirstOrDefault(x => x.Name == name);
    }

    public ProjectTaskViewModel? GetTaskById(int id)
    {
        return Tasks.FirstOrDefault(x => x.Id == id);
    }

    public async Task StartOrStop(int id, bool notification = false)
    {
        if (RunningTask?.Id == id)
        {
            if (!IsTaskRunning)
                await StartAsync(id, notification);
            else
                await StopAsync(notification);
        }
        else
        {
            await StartAsync(id, notification);
        }
    }

    private async Task StartAsync(int id, bool notification = false)
    {
        await StopAsync();

        if (_state != State.Stopped)
            return;

        var task = GetTaskById(id);
        if (task == null)
            return;

        var now = DateTime.UtcNow;
        _state = State.Running;
        RunningTask = task;
        RunningTask.TotalElapsedTime = await _store.CalculateTotalTaskElapsedTime(task.Id, TimePeriod);
        RunningTask.runStartTime = now;
        _taskTimer = StartTimer();

        if (notification)
            _notificationManager.ScheduleNotification($"⏳ {task.Name}", "");
    }

    private void StartBreak()
    {
        if (RunningTask == null || MaxBreakTime.TotalSeconds <= 0)
            return;

        var now = DateTime.UtcNow;
        var elapsedTime = RunningTask.GetRunElapsedTime(now);
        var elapsedTimeStr = TimeUtil.FormatElapsedTimePretty(elapsedTime);
        var breakTimeStr = TimeUtil.FormatElapsedTimePretty(RunningTask.runBreakEndTime - DateTime.Now);
        _notificationManager.ScheduleNotification(Title, $"{elapsedTimeStr} have elapsed. Please take a break of {breakTimeStr}.");
        RunningTask.StartBreak(now);
        _state = State.Break;
    }

    public async Task StopAsync(bool notification = false)
    {
        if (!IsTaskRunning || RunningTask == null)
            return;

        StopTimer();

        if (_state == State.Running)
        {
            var entry = RunningTask.CompleteTimeEntry(DateTime.UtcNow);
            await _store.AddTaskTimeEntryAsync(entry);
            RunningTask.History.Insert(0, entry);

            if (MaxBreakTime.TotalSeconds > 0)
            {
                StartBreak();
                return;
            }

            if (notification)
                _notificationManager.ScheduleNotification($"⏹ {RunningTask.Name}", "");
        }
        else if (_state == State.Break)
        {
            _notificationManager.ScheduleNotification(Title, $"It is time to {RunningTask.Name} again!");
        }

        RunningTask = null;
        _state = State.Stopped;
    }

    public async Task StartTimer()
    {
        if (_isTimerRunning)
            return;

        _isTimerRunning = true;

        while (_isTimerRunning)
        {
            try
            {
                await TickAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            StateHasChanged?.Invoke();
            await Task.Delay(150);
        }
    }

    public async Task HandleKeyDown(KeyboardEventArgs e)
    {
        var key = e.Key.ToLower();

        if (!e.CtrlKey && !e.AltKey && key == "delete")
        {
            await OpenDeleteTaskConfirmDialog(SelectedTaskId, SelectedTaskName);
            return;
        }

        if (!e.CtrlKey || !e.AltKey)
            return;

        switch (key)
        {
            case "t":
                await OpenCreateTaskDialog();
                break;
            case "s":
                await StartOrStop(SelectedTaskId);
                break;
            case "x":
                await ToggleCompleteStatus(SelectedTaskId);
                break;
            case "w":
                await OpenTaskEditDialog(SelectedTaskId);
                break;
            case "arrowup":
                SelectNextTask(-1);
                break;
            case "arrowdown":
                SelectNextTask(1);
                break;
            default:
                return;
        }

        StateHasChanged();
    }

    public async Task TimePeriodChanged(TimePeriod period)
    {
        TimePeriod = period;

        foreach (var task in Tasks)
        {
            var elapsedTime = await _store.CalculateTotalTaskElapsedTime(task.Id, TimePeriod);
            task.ElapsedTime = elapsedTime;
        }
    }

    public async Task ElapsedTimeChanged(int taskId)
    {
        var task = GetTaskById(taskId);
        if (task != null)
            task.ElapsedTime = await _store.CalculateTotalTaskElapsedTime(task.Id, TimePeriod); ;
    }

    private void StopTimer()
    {
        if (!_isTimerRunning)
            return;

        _isTimerRunning = false;
    }

    private async Task TickAsync()
    {
        if (RunningTask == null)
            return;

        var now = DateTime.UtcNow;

        if (_state == State.Running)
        {
            var elapsedTime = RunningTask.GetRunElapsedTime(now);
            RunningTask.ElapsedTime = RunningTask.TotalElapsedTime + elapsedTime;

            if (RunningTask.Duration > TimeSpan.Zero && elapsedTime > RunningTask.Duration)
                await CompleteTaskAsync(RunningTask);
            else if (MaxBreakTime > TimeSpan.Zero && elapsedTime > MaxBreakTime)
                await StopAsync();
        }
        else if (now >= RunningTask.runBreakEndTime)
        {
            await StopAsync();
        }
    }

    private async Task CompleteTaskAsync(ProjectTaskViewModel task, bool manual = false)
    {
        if (task.TrackTime)
        {
            if (RunningTask == task)
                await StopAsync();
        }

        if (!await _taskService.CompleteTaskAsync(task.Id, DateTime.UtcNow))
            return;

        if (!manual)
            _notificationManager.ScheduleNotification(Title, $"{task.Name} completed.");
    }

    private static ProjectTaskViewModel CreateTaskVm(ProjectTask task)
    {
        return new ProjectTaskViewModel()
        {
            Id = task.Id,
            Name = task.Name,
            ProjectId = task.Project,
            Duration = task.Duration,
            StartDate = task.StartDate,
            CompleteDate = task.CompleteDate,
            Repeat = task.Repeat,
        };
    }

    private async void TaskEventAdded(object obj)
    {
        switch (obj)
        {
            case TaskAdded taskAdded:
                Tasks.Add(CreateTaskVm(taskAdded.task));
                break;
            case TaskEdited taskEdited:
                {
                    var task = taskEdited.task;
                    var taskVm = GetTaskById(taskEdited.task.Id);
                    if (taskVm != null)
                    {
                        taskVm.Name = task.Name;
                        taskVm.Duration = task.Duration;
                    }
                }
                break;
            case TaskCompleted taskCompleted:
                {
                    var task = taskCompleted.task;
                    var taskVm = GetTaskById(task.Id);
                    if (taskVm != null)
                        taskVm.CompleteDate = task.CompleteDate;
                }
                break;
            case TaskDeleted taskDeleted:
                {
                    var task = taskDeleted.task;
                    var taskVm = GetTaskById(task.Id);
                    if (taskVm != null)
                        Tasks.Remove(taskVm);
                }
                break;
            case SelectedProjectChanged:
                await LoadTasks();
                SelectedTask = null;
                break;
            case OpenAddTaskDialogEvent:
                await OpenCreateTaskDialog();
                break;
            case ToggleStartTaskEvent:
                await StartOrStop(SelectedTaskId, notification: true);
                break;
            case TaskTimeEntryDeleted t:
                await ElapsedTimeChanged(t.taskId);
                break;
        }

        StateHasChanged();
    }
}
