using D20Tek.Notepad.Tui.Commands;

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

        // Arrow keys - Word Navigation (Ctrl)
        [(Key.CursorLeft, false, true)] = vm => vm.MoveWordLeft(),
        [(Key.CursorRight, false, true)] = vm => vm.MoveWordRight(),

        // Arrow keys - Selection
        [(Key.CursorLeft, true, false)] = vm => vm.ExtendLeft(),
        [(Key.CursorRight, true, false)] = vm => vm.ExtendRight(),
        [(Key.CursorUp, true, false)] = vm => vm.ExtendUp(),
        [(Key.CursorDown, true, false)] = vm => vm.ExtendDown(),

        // Arrow keys - Word Selection (Ctrl+Shift)
        [(Key.CursorLeft, true, true)] = vm => vm.ExtendWordLeft(),
        [(Key.CursorRight, true, true)] = vm => vm.ExtendWordRight(),

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

        // Editing - Delete Word (Ctrl)
        [(Key.Backspace, false, true)] = vm => vm.DeleteWordLeft(),
        [(Key.DeleteChar, false, true)] = vm => vm.DeleteWordRight(),

        // Editing - New Line and Tab
        [(Key.Enter, false, false)] = vm => vm.InsertNewLine(),
        [(Key.Tab, false, false)] = vm => vm.InsertTab(),
        [(Key.InsertChar, false, false)] = vm => vm.ToggleInsertMode(),

        // Undo/Redo
        [(Key.Z, false, true)] = vm => vm.Undo(),
        [(Key.Y, false, true)] = vm => vm.Redo(),
        [(Key.Z, true, true)] = vm => vm.Redo(),

        // Select All
        [(Key.A, false, true)] = vm => vm.SelectAll(),

        // Line manipulation
        [(Key.D, false, true)] = vm => vm.DuplicateLine(),

        // File commands
        [(Key.N, false, true)] = vm => FileNewCommand.Execute(vm),
        [(Key.O, false, true)] = vm => FileOpenCommand.Execute(vm),
        [(Key.E, false, true)] = vm => FileSaveCommand.Execute(vm),
        [(Key.E, true, true)] = vm => FileSaveAsCommand.Execute(vm),

        // Search commands
        [(Key.F, false, true)] = vm => FindCommand.Execute(vm),
        [(Key.H, false, true)] = vm => ReplaceCommand.Execute(vm),
        [(Key.G, false, true)] = vm => GoToLineCommand.Execute(vm),
        [(Key.F3, false, false)] = vm => FindNextCommand.Execute(vm),
        [(Key.F3, true, false)] = vm => FindPreviousCommand.Execute(vm),
    };

    public static bool TryExecute(EditorViewModel vm, KeyEvent keyEvent)
    {
        // Alt+Arrow keys for line movement (before general Alt filter)
        bool alt = (keyEvent.Key & Key.AltMask) != 0;
        if (alt)
        {
            Key altBase = keyEvent.Key & ~Key.AltMask;
            if (altBase == Key.CursorUp) { vm.MoveLineUp(); return true; }
            if (altBase == Key.CursorDown) { vm.MoveLineDown(); return true; }
            return false;
        }

        bool shift = (keyEvent.Key & Key.ShiftMask) != 0;
        bool ctrl = (keyEvent.Key & Key.CtrlMask) != 0;
        Key baseKey = keyEvent.Key & ~(Key.ShiftMask | Key.CtrlMask);

        // Shift+Tab for outdent (multi-line selection)
        if (baseKey == Key.Tab && shift && !ctrl && vm.Session.HasSelection)
        {
            vm.OutdentSelection();
            return true;
        }

        // Tab for indent when multi-line selection exists
        if (baseKey == Key.Tab && !shift && !ctrl && vm.Session.HasSelection)
        {
            var range = vm.Session.GetSelectionRange();
            if (range.Start.Line != range.End.Line)
            {
                vm.IndentSelection();
                return true;
            }
        }

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
