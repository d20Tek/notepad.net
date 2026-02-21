namespace D20Tek.Notepad.Tui;

internal static class MouseHandler
{
    private const int EdgeScrollSpeed = 3;

    public static bool ProcessMouse(EditorViewModel vm, MouseEvent mouseEvent)
    {
        var flags = mouseEvent.Flags;

        // Scroll up
        if (flags.HasFlag(MouseFlags.WheeledUp))
        {
            vm.ScrollLines(-3);
            Application.Driver.SetCursorVisibility(CursorVisibility.Invisible);
            return true;
        }

        // Scroll down
        if (flags.HasFlag(MouseFlags.WheeledDown))
        {
            vm.ScrollLines(3);
            Application.Driver.SetCursorVisibility(CursorVisibility.Invisible);
            return true;
        }

        // --- Mouse Drag: extend selection (check BEFORE Button1Pressed alone) ---
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
        vm.SetAnchorToCaret();
        var (line, col) = MouseToDocumentPosition(vm, me);
        vm.MoveCaretTo(line, col);
        vm.EnsureCaretVisible();
    }

    private static void BeginMouseSelection(EditorViewModel vm, MouseEvent me)
    {
        var (line, col) = MouseToDocumentPosition(vm, me);
        vm.SetAnchorToCaret();
        vm.MoveCaretTo(line, col);
        vm.EnsureCaretVisible();
    }

    private static void UpdateMouseSelection(EditorViewModel vm, MouseEvent me)
    {
        // Edge-scroll when dragging near edges of viewport
        EdgeScrollVertical(vm, me);
        EdgeScrollHorizontal(vm, me);

        var (line, col) = MouseToDocumentPosition(vm, me);
        vm.ExtendSelectionTo(line, col);
        vm.Refresh();
    }

    private static void EdgeScrollVertical(EditorViewModel vm, MouseEvent me)
    {
        int viewportHeight = vm.Viewport.VisibleLineCount;
        int totalLines = vm.Session.Document.Lines.Count;

        if (me.Y <= 0 && vm.Viewport.FirstVisibleLine > 0)
        {
            // Dragging at or above the top edge - scroll up
            vm.ScrollLines(-EdgeScrollSpeed);
        }
        else if (me.Y >= viewportHeight - 1 &&
                 vm.Viewport.FirstVisibleLine + viewportHeight < totalLines)
        {
            // Dragging at or below the bottom edge - scroll down
            vm.ScrollLines(EdgeScrollSpeed);
        }
    }

    private static void EdgeScrollHorizontal(EditorViewModel vm, MouseEvent me)
    {
        int viewportWidth = vm.ViewportWidth;

        if (me.X <= 0 && vm.Viewport.HorizontalOffset > 0)
        {
            // Dragging at or left of the left edge - scroll left
            vm.ScrollColumns(-20);
        }
        else if (me.X >= viewportWidth - 1)
        {
            // Dragging at or right of the right edge - scroll right
            vm.ScrollColumns(20);
        }
    }

    private static void EndMouseSelection(EditorViewModel vm, MouseEvent me)
    {
        var (line, col) = MouseToDocumentPosition(vm, me);
        vm.ExtendSelectionTo(line, col);
        vm.EnsureCaretVisible();
    }

    private static (int line, int col) MouseToDocumentPosition(EditorViewModel vm, MouseEvent me)
    {
        int viewportHeight = vm.Viewport.VisibleLineCount;
        int viewportWidth = vm.ViewportWidth;

        // Calculate line, handling positions outside viewport
        int line;
        if (me.Y < 0)
        {
            // Above viewport - target first visible line
            line = vm.Viewport.FirstVisibleLine;
        }
        else if (me.Y >= viewportHeight)
        {
            // Below viewport - target last visible line
            line = vm.Viewport.FirstVisibleLine + viewportHeight - 1;
        }
        else
        {
            line = me.Y + vm.Viewport.FirstVisibleLine;
        }
        line = Math.Clamp(line, 0, vm.Session.Document.Lines.Count - 1);

        // Calculate column, handling positions outside viewport
        int lineLength = vm.GetLineLength(line);
        int col;
        if (me.X < 0)
        {
            // Left of viewport
            col = Math.Max(0, vm.Viewport.HorizontalOffset - 1);
        }
        else if (me.X >= viewportWidth)
        {
            // Right of viewport
            col = Math.Min(lineLength, vm.Viewport.HorizontalOffset + viewportWidth);
        }
        else
        {
            col = me.X + vm.Viewport.HorizontalOffset;
        }
        col = Math.Clamp(col, 0, lineLength);

        return (line, col);
    }
}
