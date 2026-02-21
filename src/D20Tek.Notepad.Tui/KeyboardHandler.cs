using D20Tek.Notepad.Tui.Input;

namespace D20Tek.Notepad.Tui;

/// <summary>
/// Handles keyboard input by delegating to KeyBindings.
/// </summary>
internal static class KeyboardHandler
{
    public static bool ProcessKey(EditorViewModel vm, KeyEvent keyEvent) =>
        KeyBindings.TryExecute(vm, keyEvent);
}
