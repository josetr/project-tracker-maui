namespace ProjectTracker.WinUI;

using ProjectTracker.Models;
using static ProjectTracker.WinUI.Interop.User32;

public static partial class Mapping
{
    public static Modifiers ToModifiers(this KeyModifiers modifiers)
    {
        Modifiers result = 0;

        if (modifiers.HasFlag(KeyModifiers.Control))
            result |= Modifiers.MOD_CONTROL;

        if (modifiers.HasFlag(KeyModifiers.Menu))
            result |= Modifiers.MOD_ALT;

        if (modifiers.HasFlag(KeyModifiers.Shift))
            result |= Modifiers.MOD_SHIFT;

        if (modifiers.HasFlag(KeyModifiers.Windows))
            result |= Modifiers.MOD_WIN;

        return result;
    }

    public static KeyModifiers ToKeyModifiers(this Modifiers key)
    {
        KeyModifiers result = 0;
        if (key.HasFlag(Modifiers.MOD_CONTROL))
            result |= KeyModifiers.Control;

        if (key.HasFlag(Modifiers.MOD_ALT))
            result |= KeyModifiers.Menu;

        if (key.HasFlag(Modifiers.MOD_SHIFT))
            result |= KeyModifiers.Shift;

        if (key.HasFlag(Modifiers.MOD_WIN))
            result |= KeyModifiers.Windows;

        return (KeyModifiers)key;
    }
}
