namespace ProjectTracker.WinUI;

using System.Collections.Concurrent;
using ProjectTracker.Models;
using static ProjectTracker.WinUI.Interop.User32;

public sealed class HotkeyManager : IHotkeyManager, IDisposable
{
    private BlockingCollection<Action> requests = new();
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private Action<Hotkey> _handler = (h) => { };
    private Thread? thread;


    public void Dispose()
    {
        cancellationTokenSource.Cancel();
        if (thread != null)
            thread.Join();
    }

    public void RegisterHotkey(int id, Hotkey hotkey)
    {
        if (thread == null)
        {
            var cancellationToken = cancellationTokenSource.Token;
            thread = new Thread(() =>
            {
                MSG msg = default;

                while (!cancellationToken.IsCancellationRequested)
                {
                    Thread.Sleep(50);

                    while (requests.TryTake(out var action))
                        action();

                    if (!(PeekMessage(ref msg, IntPtr.Zero, WM_HOTKEY, WM_HOTKEY, PM_REMOVE)))
                        continue;

                    var key = (Key)HiWord(msg.lParam);
                    var modifiers = ((Modifiers)(short)msg.lParam).ToKeyModifiers();
                    App.SetForegroundWindow();

                    _handler?.Invoke(new Hotkey()
                    {
                        Key = key,
                        Modifiers = modifiers
                    });
                }

                while (requests.TryTake(out var action))
                    action();
            });
            thread.Start();
        }

        requests.Add(() =>
        {
            if (hotkey != null)
            {
                var result = RegisterHotKey(IntPtr.Zero, id, hotkey.Modifiers.ToModifiers(), (uint)hotkey.Key);
                Console.WriteLine(result);
            }
        });
    }

    public void UnregisterHotkey(int id)
    {
        requests.Add(() =>
        {
            while (UnregisterHotKey(IntPtr.Zero, id))
            {
            }
        });
    }

    public void RegisterHandler(Action<Hotkey> handler)
    {
        _handler = handler;
    }

    private static uint HiWord(IntPtr ptr)
    {
        uint value = (uint)(int)ptr;
        if ((value & 0x80000000) == 0x80000000)
            return value >> 16;
        else
            return (value >> 16) & 0xffff;
    }
}
