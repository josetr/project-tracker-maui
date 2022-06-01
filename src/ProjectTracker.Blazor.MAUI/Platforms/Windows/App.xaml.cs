using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using ProjectTracker.WinUI.Interop;
using WinRT.Interop;

namespace ProjectTracker.WinUI;

public partial class App : MauiWinUIApplication
{
    public App()
    {
        this.InitializeComponent();
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    public static IntPtr hWnd;

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        Platform.OnLaunched(args);

        var currentWindow = Application.Windows[0].Handler?.PlatformView;
        hWnd = WindowNative.GetWindowHandle(currentWindow);
        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);
        appWindow.Title = "Project Tracker";
    }

    public static void SetForegroundWindow()
    {
        User32.SetForegroundWindow(hWnd);
    }
}
