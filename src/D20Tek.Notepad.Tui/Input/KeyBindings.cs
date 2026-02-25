namespace D20Tek.Notepad.Tui.Input;

internal static class KeyBindings
{
    private static readonly Dictionary<(Key key, bool shift, bool ctrl), Action<EditorViewModel>> _bindings = new()
    {
        // Arrow keys - Movement
        [(Key.CursorLeft, false, false)] = vm => vm.MoveLeft(),
        [(Key.CursorRight, false, false)] = vm => vm.MoveRight(),
        [(Key.CursorUp, false, false)] = vm => vm.MoveUp(),
        [(Key.CursorDown, false, false)] = vm => vm.MoveDown(),

        // Arrow keys - Selection
        [(Key.CursorLeft, true, false)] = vm => vm.ExtendLeft(),
        [(Key.CursorRight, true, false)] = vm => vm.ExtendRight(),
        [(Key.CursorUp, true, false)] = vm => vm.ExtendUp(),
        [(Key.CursorDown, true, false)] = vm => vm.ExtendDown(),

        // Home/End - Line
        [(Key.Home, false, false)] = vm => vm.MoveToLineStart(),
        [(Key.End, false, false)] = vm => vm.MoveToLineEnd(),
        [(Key.Home, true, false)] = vm => vm.ExtendToLineStart(),
        [(Key.End, true, false)] = vm => vm.ExtendToLineEnd(),

        // Home/End - Document (Ctrl)
        [(Key.Home, false, true)] = vm => vm.MoveToDocumentStart(),
        [(Key.End, false, true)] = vm => vm.MoveToDocumentEnd(),
        [(Key.Home, true, true)] = vm => vm.ExtendToDocumentStart(),
        [(Key.End, true, true)] = vm => vm.ExtendToDocumentEnd(),

        // Page Up/Down - Movement
        [(Key.PageUp, false, false)] = vm => vm.MovePageUp(),
        [(Key.PageDown, false, false)] = vm => vm.MovePageDown(),

        // Page Up/Down - Selection
        [(Key.PageUp, true, false)] = vm => vm.ExtendPageUp(),
        [(Key.PageDown, true, false)] = vm => vm.ExtendPageDown(),

        // Editing - Deletion
        [(Key.Backspace, false, false)] = vm => vm.Backspace(),
        [(Key.DeleteChar, false, false)] = vm => vm.Delete(),

        // Editing - New Line and Tab
        [(Key.Enter, false, false)] = vm => vm.InsertNewLine(),
        [(Key.Tab, false, false)] = vm => vm.InsertTab(),

        // Undo/Redo
        [(Key.Z, false, true)] = vm => vm.Undo(),
        [(Key.Y, false, true)] = vm => vm.Redo(),
        [(Key.Z, true, true)] = vm => vm.Redo(),

        // Select All
        [(Key.A, false, true)] = vm => vm.SelectAll(),
    };

    public static bool TryExecute(EditorViewModel vm, KeyEvent keyEvent)
    {
        bool shift = (keyEvent.Key & Key.ShiftMask) != 0;
        bool ctrl = (keyEvent.Key & Key.CtrlMask) != 0;
        Key baseKey = keyEvent.Key & ~(Key.ShiftMask | Key.CtrlMask);

        if (_bindings.TryGetValue((baseKey, shift, ctrl), out var action))
        {
            action(vm);
            return true;
        }

        // Handle printable character input
        if (TryHandleCharacterInput(vm, keyEvent))
        {
            return true;
        }

        return false;
    }

    private static bool TryHandleCharacterInput(EditorViewModel vm, KeyEvent keyEvent)
    {
        if ((keyEvent.Key & Key.CtrlMask) != 0) return false;

        // Check if it's a printable character (space through tilde, plus extended chars)
        char c = (char)keyEvent.KeyValue;
        if (c >= ' ' && c <= '~' || char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSymbol(c))
        {
            vm.TypeCharacter(c);
            return true;
        }

        return false;
    }
}
