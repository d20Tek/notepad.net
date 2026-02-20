namespace D20Tek.Notepad.Tui;

internal static class MouseHandler
{
    public static bool ProcessMouse(EditorViewModel vm, MouseEvent mouseEvent)
    {
        var flags = mouseEvent.Flags;

        // Scroll up
        if (flags.HasFlag(MouseFlags.WheeledUp))
        {
            vm.ScrollLines(-3);
            return true;
        }

        // Scroll down
        if (flags.HasFlag(MouseFlags.WheeledDown))
        {
            vm.ScrollLines(3);
            return true;
        }

        // --- Mouse Drag: extend selection (check BEFORE Button1Pressed alone) ---
        // Drag events have both Button1Pressed AND ReportMousePosition
        if (flags.HasFlag(MouseFlags.Button1Pressed) && flags.HasFlag(MouseFlags.ReportMousePosition))
        {
            UpdateMouseSelection(vm, mouseEvent);
            return true;
        }

        // --- Mouse Down: start selection (only when NOT dragging) ---
        if (flags.HasFlag(MouseFlags.Button1Pressed))
        {
            BeginMouseSelection(vm, mouseEvent);
            return true;
        }

        // --- Mouse Up: finalize selection ---
        if (flags.HasFlag(MouseFlags.Button1Released))
        {
            EndMouseSelection(vm, mouseEvent);
            return true;
        }

        // Handle left-click
        if (flags.HasFlag(MouseFlags.Button1Clicked))
        {
            MoveCaretFromMouse(vm, mouseEvent);
            return true;
        }

        return false;
    }

    private static void MoveCaretFromMouse(EditorViewModel vm, MouseEvent me)
    {
        var (line, col) = MouseToDocumentPosition(vm, me);

        bool shift = me.Flags.HasFlag(MouseFlags.ButtonShift);

        if (shift)
        {
            vm.ExtendSelectionTo(line, col);
        }
        else
        {
            vm.MoveCaretTo(line, col);
        }
        vm.Refresh();
    }

    private static void BeginMouseSelection(EditorViewModel vm, MouseEvent me)
    {
        var (line, col) = MouseToDocumentPosition(vm, me);
        vm.MoveCaretTo(line, col);
        vm.SetAnchorToCaret();
        vm.Refresh();
    }

    private static void UpdateMouseSelection(EditorViewModel vm, MouseEvent me)
    {
        var (line, col) = MouseToDocumentPosition(vm, me);
        vm.ExtendSelectionTo(line, col);
        vm.Refresh();
    }

    private static void EndMouseSelection(EditorViewModel vm, MouseEvent me)
    {
        var (line, col) = MouseToDocumentPosition(vm, me);
        vm.ExtendSelectionTo(line, col);
        vm.Refresh();
    }

    private static (int line, int col) MouseToDocumentPosition(EditorViewModel vm, MouseEvent me)
    {
        int line = me.Y + vm.Viewport.FirstVisibleLine;
        line = Math.Max(0, Math.Min(line, vm.Session.Document.Lines.Count - 1));

        int col = me.X + vm.Viewport.HorizontalOffset;
        int lineLength = vm.GetLineLength(line);
        col = Math.Max(0, Math.Min(col, lineLength));

        return (line, col);
    }
}
