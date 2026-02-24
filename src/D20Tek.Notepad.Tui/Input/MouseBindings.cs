namespace D20Tek.Notepad.Tui.Input;

internal static class MouseBindings
{
    public static bool TryExecute(EditorViewModel vm, MouseEvent mouseEvent)
    {
        var flags = mouseEvent.Flags;

        // Wheel scroll (highest priority)
        if (flags.HasFlag(MouseFlags.WheeledUp))
        {
            ScrollWheel(vm, -vm.Settings.MouseWheelScrollLines);
            return true;
        }

        if (flags.HasFlag(MouseFlags.WheeledDown))
        {
            ScrollWheel(vm, vm.Settings.MouseWheelScrollLines);
            return true;
        }

        // Mouse drag: extend selection (check BEFORE Button1Pressed alone)
        if (flags.HasFlag(MouseFlags.Button1Pressed) && flags.HasFlag(MouseFlags.ReportMousePosition))
        {
            DragSelection(vm, mouseEvent);
            return true;
        }

        // Mouse down: start selection
        if (flags.HasFlag(MouseFlags.Button1Pressed))
        {
            BeginSelection(vm, mouseEvent);
            return true;
        }

        // Mouse up: finalize selection
        if (flags.HasFlag(MouseFlags.Button1Released))
        {
            EndSelection(vm, mouseEvent);
            return true;
        }

        // Left-click: move caret
        if (flags.HasFlag(MouseFlags.Button1Clicked))
        {
            Click(vm, mouseEvent);
            return true;
        }

        return false;
    }

    // Mouse Actions
    private static void ScrollWheel(EditorViewModel vm, int delta)
    {
        vm.ScrollLines(delta);
        Application.Driver.SetCursorVisibility(CursorVisibility.Invisible);
    }

    private static void Click(EditorViewModel vm, MouseEvent me)
    {
        vm.SetAnchorToCaret();
        var (line, col) = ToDocumentPosition(vm, me);
        vm.MoveCaretTo(line, col);
    }

    private static void BeginSelection(EditorViewModel vm, MouseEvent me)
    {
        var (line, col) = ToDocumentPosition(vm, me);
        vm.SetAnchorToCaret();
        vm.MoveCaretTo(line, col);
    }

    private static void DragSelection(EditorViewModel vm, MouseEvent me)
    {
        EdgeScroll(vm, me);

        var (line, col) = ToDocumentPosition(vm, me);
        vm.ExtendSelectionTo(line, col);
        vm.Refresh();
    }

    private static void EndSelection(EditorViewModel vm, MouseEvent me)
    {
        var (line, col) = ToDocumentPosition(vm, me);
        vm.ExtendSelectionTo(line, col);
    }

    // Edge Scrolling
    private static void EdgeScroll(EditorViewModel vm, MouseEvent me)
    {
        EdgeScrollVertical(vm, me);
        EdgeScrollHorizontal(vm, me);
    }

    private static void EdgeScrollVertical(EditorViewModel vm, MouseEvent me)
    {
        int viewportHeight = vm.Viewport.VisibleLineCount;
        int totalLines = vm.GetTotalVisualLineCount();
        int speed = vm.Settings.EdgeScrollSpeed;

        if (me.Y <= 0 && vm.Viewport.FirstVisibleLine > 0)
        {
            vm.ScrollLines(-speed);
        }
        else if (me.Y >= viewportHeight - 1 &&
                 vm.Viewport.FirstVisibleLine + viewportHeight < totalLines)
        {
            vm.ScrollLines(speed);
        }
    }

    private static void EdgeScrollHorizontal(EditorViewModel vm, MouseEvent me)
    {
        int viewportWidth = vm.ViewportWidth;
        int amount = vm.Settings.HorizontalEdgeScrollAmount;

        if (me.X <= 0 && vm.Viewport.HorizontalOffset > 0)
        {
            vm.ScrollColumns(-amount);
        }
        else if (me.X >= viewportWidth - 1)
        {
            vm.ScrollColumns(amount);
        }
    }

    // Position Conversion
    public static (int line, int column) ToDocumentPosition(EditorViewModel vm, MouseEvent me)
    {
        if (vm.IsWordWrapEnabled)
        {
            return ToDocumentPositionWrapped(vm, me);
        }

        int line = CalculateLine(vm, me.Y);
        int column = CalculateColumn(vm, me.X, line);
        return (line, column);
    }

    private static (int line, int column) ToDocumentPositionWrapped(EditorViewModel vm, MouseEvent me)
    {
        var visibleLines = vm.VisibleLines;
        int viewLineIndex = Math.Clamp(me.Y, 0, visibleLines.Count - 1);

        if (visibleLines.Count == 0)
        {
            return (0, 0);
        }

        var viewLine = visibleLines[viewLineIndex];
        int docLine = viewLine.DocumentLineIndex;
        int segmentStart = viewLine.SegmentStartColumn;

        // Calculate column within the segment, then add segment offset
        int columnInSegment = Math.Clamp(me.X, 0, viewLine.Text.Length);
        int docColumn = segmentStart + columnInSegment;

        // Clamp to actual line length
        int lineLength = vm.GetLineLength(docLine);
        docColumn = Math.Clamp(docColumn, 0, lineLength);

        return (docLine, docColumn);
    }

    private static int CalculateLine(EditorViewModel vm, int mouseY)
    {
        int viewportHeight = vm.Viewport.VisibleLineCount;
        int firstVisible = vm.Viewport.FirstVisibleLine;
        int totalLines = vm.Session.Document.Lines.Count;

        int line;
        if (mouseY < 0)
        {
            line = firstVisible;
        }
        else if (mouseY >= viewportHeight)
        {
            line = firstVisible + viewportHeight - 1;
        }
        else
        {
            line = mouseY + firstVisible;
        }

        return Math.Clamp(line, 0, totalLines - 1);
    }

    private static int CalculateColumn(EditorViewModel vm, int mouseX, int line)
    {
        int viewportWidth = vm.ViewportWidth;
        int horizontalOffset = vm.Viewport.HorizontalOffset;
        int lineLength = vm.GetLineLength(line);

        int column;
        if (mouseX < 0)
        {
            column = Math.Max(0, horizontalOffset - 1);
        }
        else if (mouseX >= viewportWidth)
        {
            column = Math.Min(lineLength, horizontalOffset + viewportWidth);
        }
        else
        {
            column = mouseX + horizontalOffset;
        }

        return Math.Clamp(column, 0, lineLength);
    }
}
