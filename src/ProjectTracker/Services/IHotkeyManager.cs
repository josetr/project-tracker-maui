namespace ProjectTracker;

using ProjectTracker.Models;

public interface IHotkeyManager
{
    void RegisterHotkey(int id, Hotkey hotkey);
    void UnregisterHotkey(int id);
    void RegisterHandler(Action<Hotkey> handler);
}
