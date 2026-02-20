namespace D20Tek.Notepad.Tui;

internal static class KeyProcessorService
{
    public static bool ProcessKey(EditorViewModel vm, KeyEvent keyEvent)
    {
        bool shift = (keyEvent.Key & Key.ShiftMask) != 0;
        bool ctrl = (keyEvent.Key & Key.CtrlMask) != 0;

        // Strip modifiers so the switch sees only the base key
        Key key = keyEvent.Key & ~(Key.ShiftMask | Key.CtrlMask);

        switch (key)
        {
            // ----------------------------------------------------
            // Arrow Keys
            // ----------------------------------------------------
            case Key.CursorLeft:
                if (shift) vm.Navigator.ExtendSelectionLeft();
                else vm.Navigator.MoveCaretLeft();
                return true;

            case Key.CursorRight:
                if (shift) vm.Navigator.ExtendSelectionRight();
                else vm.Navigator.MoveCaretRight();
                return true;

            case Key.CursorUp:
                if (shift) vm.Navigator.ExtendSelectionUp();
                else vm.Navigator.MoveCaretUp();
                return true;

            case Key.CursorDown:
                if (shift) vm.Navigator.ExtendSelectionDown();
                else vm.Navigator.MoveCaretDown();
                return true;

            // ----------------------------------------------------
            // Home / End
            // ----------------------------------------------------
            case Key.Home:
                if (ctrl)
                {
                    if (shift) vm.Navigator.ExtendSelectionToDocumentStart();
                    else vm.Navigator.MoveToDocumentStart();
                }
                else
                {
                    if (shift) vm.Navigator.ExtendSelectionToLineStart();
                    else vm.Navigator.MoveToLineStart();
                }
                return true;

            case Key.End:
                if (ctrl)
                {
                    if (shift) vm.Navigator.ExtendSelectionToDocumentEnd();
                    else vm.Navigator.MoveToDocumentEnd();
                }
                else
                {
                    if (shift) vm.Navigator.ExtendSelectionToLineEnd();
                    else vm.Navigator.MoveToLineEnd();
                }
                return true;

            // ----------------------------------------------------
            // Page Up / Page Down
            // ----------------------------------------------------
            case Key.PageUp:
                if (shift) vm.Navigator.ExtendSelectionPageUp();
                else vm.Navigator.MovePageUp();
                return true;

            case Key.PageDown:
                if (shift) vm.Navigator.ExtendSelectionPageDown();
                else vm.Navigator.MovePageDown();
                return true;
        }

        return false;
    }
}
