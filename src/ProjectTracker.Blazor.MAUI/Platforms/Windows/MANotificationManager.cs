namespace ProjectTracker;

using NotificationsExtensions.Toasts;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

public partial class MANotificationManager : INotificationManager
{
#pragma warning disable CS0067
    public event EventHandler NotificationReceived = default!;
#pragma warning restore CS0067

    public void Initialize()
    {
    }

    public void ReceiveNotification(string title, string message)
    {
    }

    [Obsolete]
    public int ScheduleNotification(string title, string message)
    {
        var content = new ToastContent()
        {
            Visual = new ToastVisual()
            {
                TitleText = title != null ? new ToastText() { Text = title } : null,
                BodyTextLine1 = message != null ? new ToastText() { Text = message } : null,
            },
            Audio = new ToastAudio()
            {
                Silent = true,
            }
        };

        var xmlContent = new XmlDocument();
        xmlContent.LoadXml(content.GetContent());
        ToastNotificationManager.History.Clear();
        ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(xmlContent));
        return 0;
    }
}
