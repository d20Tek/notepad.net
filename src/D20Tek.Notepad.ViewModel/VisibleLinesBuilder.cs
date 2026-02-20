namespace D20Tek.Notepad.ViewModel;

internal static class VisibleLinesBuilder
{
    public static List<ViewLine> Build(EditorViewModel viewModel, int viewportWidth)
    {
        var first = viewModel.Viewport.FirstVisibleLine;
        var count = viewModel.Viewport.VisibleLineCount;
        var visibleLines = new List<ViewLine>(count);
        var total = viewModel.Session.Document.Lines.Count;
        int lastExclusive = Math.Min(first + count, total);     // last visible line index

        int hOffset = viewModel.Viewport.HorizontalOffset;

        for (int docLine = first; docLine < lastExclusive; docLine++)
        {
            var fullText = viewModel.Session.Document.Lines[docLine].Content;

            string sliced;
            if (viewportWidth <= 0)
            {
                // no horizontal limit - return full text (minus offset)
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

            visibleLines.Add(new ViewLine(docLine, sliced));
        }

        return visibleLines;
    }

    public static ViewPosition MapCaret(EditorViewModel viewModel) => new(
        Math.Max(0, viewModel.Session.Caret.Line - viewModel.Viewport.FirstVisibleLine),
        Math.Max(0, viewModel.Session.Caret.Column - viewModel.Viewport.HorizontalOffset));

    public static SelectionViewRange? MapSelection(EditorViewModel viewModel)
    {
        if (!viewModel.Session.HasSelection) return null;

        var raw = ViewMapping.DocumentSelectionToView(
            viewModel.Session.Anchor,
            viewModel.Session.Caret,
            viewModel.Viewport.FirstVisibleLine,
            viewModel.Viewport.VisibleLineCount);

        if (raw is null) return null;

        return new SelectionViewRange(
            new ViewPosition(raw.Value.Start.LineIndex, Math.Max(0, raw.Value.Start.Column - viewModel.Viewport.HorizontalOffset)),
            new ViewPosition(raw.Value.End.LineIndex, Math.Max(0, raw.Value.End.Column - viewModel.Viewport.HorizontalOffset))
            ).Normalize();
    }
}
