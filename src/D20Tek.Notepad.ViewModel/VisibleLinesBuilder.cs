namespace D20Tek.Notepad.ViewModel;

internal static class VisibleLinesBuilder
{
    public static List<ViewLine> Build(EditorViewModel viewModel, int viewportWidth)
    {
        return viewModel.Settings.WordWrapEnabled
            ? BuildWrapped(viewModel, viewportWidth)
            : BuildNonWrapped(viewModel, viewportWidth);
    }

    private static List<ViewLine> BuildNonWrapped(EditorViewModel viewModel, int viewportWidth)
    {
        var first = viewModel.Viewport.FirstVisibleLine;
        var count = viewModel.Viewport.VisibleLineCount;
        var visibleLines = new List<ViewLine>(count);
        var total = viewModel.Session.Document.Lines.Count;
        int lastExclusive = Math.Min(first + count, total);

        int hOffset = viewModel.Viewport.HorizontalOffset;

        for (int docLine = first; docLine < lastExclusive; docLine++)
        {
            var fullText = viewModel.Session.Document.Lines[docLine].Content;

            string sliced;
            if (viewportWidth <= 0)
            {
                sliced = hOffset < fullText.Length ? fullText[hOffset..] : string.Empty;
            }
            else if (hOffset < fullText.Length)
            {
                sliced = fullText.Substring(hOffset, Math.Min(viewportWidth, fullText.Length - hOffset));
            }
            else
            {
                sliced = string.Empty;
            }

            visibleLines.Add(new ViewLine(docLine, hOffset, sliced));
        }

        return visibleLines;
    }

    private static List<ViewLine> BuildWrapped(EditorViewModel viewModel, int viewportWidth)
    {
        var visibleLineCount = viewModel.Viewport.VisibleLineCount;
        var visibleLines = new List<ViewLine>(visibleLineCount);
        var total = viewModel.Session.Document.Lines.Count;

        if (viewportWidth <= 0 || total == 0) return visibleLines;

        int firstVisualLine = viewModel.Viewport.FirstVisibleLine;
        int visualLineIndex = 0;

        for (int docLine = 0; docLine < total && visibleLines.Count < visibleLineCount; docLine++)
        {
            var lineText = viewModel.Session.Document.Lines[docLine].Content;
            var segments = WordWrapCalculator.WrapLine(lineText, viewportWidth);
            int nextVisualIndex = visualLineIndex + segments.Count;

            // Skip document lines entirely before the visible area
            if (nextVisualIndex <= firstVisualLine)
            {
                visualLineIndex = nextVisualIndex;
                continue;
            }

            int startSegment = Math.Max(0, firstVisualLine - visualLineIndex);
            for (int i = startSegment; i < segments.Count && visibleLines.Count < visibleLineCount; i++)
            {
                visibleLines.Add(new ViewLine(docLine, segments[i].StartColumn, segments[i].Text));
            }

            visualLineIndex = nextVisualIndex;
        }


        return visibleLines;
    }

    public static ViewPosition MapCaret(EditorViewModel viewModel)
    {
        if (viewModel.Settings.WordWrapEnabled)
        {
            return ViewMappingHelper.ToWrappedViewPosition(
                viewModel.Session.Caret,
                viewModel.VisibleLines,
                viewModel.Viewport.FirstVisibleLine);
        }

        return new ViewPosition(
            Math.Max(0, viewModel.Session.Caret.Line - viewModel.Viewport.FirstVisibleLine),
            Math.Max(0, viewModel.Session.Caret.Column - viewModel.Viewport.HorizontalOffset));
    }

    public static SelectionViewRange? MapSelection(EditorViewModel viewModel)
    {
        if (!viewModel.Session.HasSelection) return null;

        if (viewModel.Settings.WordWrapEnabled)
        {
            return MapWrappedSelection(viewModel);
        }

        var raw = viewModel.Session.Anchor.ToViewSelectionRange(
            viewModel.Session.Caret,
            viewModel.Viewport.FirstVisibleLine,
            viewModel.Viewport.VisibleLineCount);

        if (raw is null) return null;

        return new SelectionViewRange(
            new ViewPosition(raw.Value.Start.LineIndex, Math.Max(0, raw.Value.Start.Column - viewModel.Viewport.HorizontalOffset)),
            new ViewPosition(raw.Value.End.LineIndex, Math.Max(0, raw.Value.End.Column - viewModel.Viewport.HorizontalOffset))
            ).Normalize();
    }

    private static SelectionViewRange? MapWrappedSelection(EditorViewModel viewModel)
    {
        var anchor = viewModel.Session.Anchor;
        var caret = viewModel.Session.Caret;
        var visibleLines = viewModel.VisibleLines;
        var firstVisibleLine = viewModel.Viewport.FirstVisibleLine;

        var anchorViewPos = ViewMappingHelper.ToWrappedViewPosition(anchor, visibleLines, firstVisibleLine);
        var caretViewPos = ViewMappingHelper.ToWrappedViewPosition(caret, visibleLines, firstVisibleLine);

        return new SelectionViewRange(anchorViewPos, caretViewPos).Normalize();
    }
}
