namespace ProjectTracker;

using System;

public class EmptyNotificationManager : INotificationManager
{
    public event EventHandler NotificationReceived = default!;

    public void Initialize()
    {

    }

    public void ReceiveNotification(string title, string message)
    {
    }

    public int ScheduleNotification(string title, string message)
    {
        return 0;
    }
}
