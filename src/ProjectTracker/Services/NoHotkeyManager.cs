namespace ProjectTracker;

using ProjectTracker.Models;

public class NoHotkeyManager : IHotkeyManager
{
    public void RegisterHandler(Action<Hotkey> handler)
    {

    }

    public void RegisterHotkey(int id, Hotkey hotkey)
    {

    }

    public void UnregisterHotkey(int id)
    {
    }
}
