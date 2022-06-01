namespace ProjectTracker.WinUI.Interop;

using System.Runtime.InteropServices;

public sealed partial class User32
{
    public enum Modifiers
    {
        MOD_ALT = 1,
        MOD_CONTROL = 2,
        MOD_SHIFT = 4,
        MOD_WIN = 8,
        MOD_NOREPEAT = 0x4000,
    }

    public static uint WM_HOTKEY = 0x0312;
    public static uint PM_REMOVE = 0x0001;

    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam;
        public IntPtr lParam;
        public int time;
        public POINT pt;
        public int lPrivate;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, Modifiers fsModifiers, uint vk);
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll")]
    internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32")]
    public static extern bool PeekMessage(ref MSG lpMsg, IntPtr handle, uint mMsgFilterInMain, uint mMsgFilterMax, uint wRemoveMsg);
}
