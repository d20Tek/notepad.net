using D20Tek.Notepad.Core.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel
{
    // Navigation (delegates to CaretNavigator with viewport awareness)
    public void MoveLeft() { Session.Navigator.MoveLeft(); EnsureCaretVisible(); }

    public void MoveRight() { Session.Navigator.MoveRight(); EnsureCaretVisible(); }

    public void MoveUp()
    {
        if (Settings.WordWrapEnabled)
        {
            MoveUpWrapped(resetAnchor: true);
        }
        else
        {
            Session.Navigator.MoveUp();
        }
        EnsureCaretVisible();
    }

    public void MoveDown()
    {
        if (Settings.WordWrapEnabled)
        {
            MoveDownWrapped(resetAnchor: true);
        }
        else
        {
            Session.Navigator.MoveDown();
        }
        EnsureCaretVisible();
    }

    public void MovePageUp() { Session.Navigator.MovePageUp(Viewport.VisibleLineCount); EnsureCaretVisible(); }

    public void MovePageDown() { Session.Navigator.MovePageDown(Viewport.VisibleLineCount); EnsureCaretVisible(); }

    public void MoveToLineStart()
    {
        // Notepad behavior: Home always goes to document line start
        Session.Navigator.MoveToLineStart();
        EnsureCaretVisible();
    }

    public void MoveToLineEnd()
    {
        // Notepad behavior: End always goes to document line end
        Session.Navigator.MoveToLineEnd();
        EnsureCaretVisible();
    }

    public void MoveToDocumentStart() { Session.Navigator.MoveToDocumentStart(); EnsureCaretVisible(); }

    public void MoveToDocumentEnd() { Session.Navigator.MoveToDocumentEnd(); EnsureCaretVisible(); }

    // Selection Extension (delegates to CaretNavigator with viewport awareness)
    public void ExtendLeft() { Session.Navigator.ExtendLeft(); EnsureCaretVisible(); }

    public void ExtendRight() { Session.Navigator.ExtendRight(); EnsureCaretVisible(); }

    public void ExtendUp()
    {
        if (Settings.WordWrapEnabled)
        {
            MoveUpWrapped(resetAnchor: false);
        }
        else
        {
            Session.Navigator.ExtendUp();
        }
        EnsureCaretVisible();
    }

    public void ExtendDown()
    {
        if (Settings.WordWrapEnabled)
        {
            MoveDownWrapped(resetAnchor: false);
        }
        else
        {
            Session.Navigator.ExtendDown();
        }
        EnsureCaretVisible();
    }

    public void ExtendPageUp() { Session.Navigator.ExtendPageUp(Viewport.VisibleLineCount); EnsureCaretVisible(); }

    public void ExtendPageDown() { Session.Navigator.ExtendPageDown(Viewport.VisibleLineCount); EnsureCaretVisible(); }

    public void ExtendToLineStart() { Session.Navigator.ExtendToLineStart(); EnsureCaretVisible(); }

    public void ExtendToLineEnd() { Session.Navigator.ExtendToLineEnd(); EnsureCaretVisible(); }

    public void ExtendToDocumentStart() { Session.Navigator.ExtendToDocumentStart(); EnsureCaretVisible(); }

    public void ExtendToDocumentEnd() { Session.Navigator.ExtendToDocumentEnd(); EnsureCaretVisible(); }

    // Caret Positioning Helpers
    public int GetLineLength(int lineIndex) => Session.GetLineLength(lineIndex);

    public void SetAnchorToCaret() => Session.Anchor = Session.Caret;

    public void MoveCaretTo(int line, int column)
    {
        var newPos = Session.ClampToDocument(line, column);
        Session.Caret = newPos;
        Session.Anchor = newPos;
    }

    // Word Wrap Navigation Helpers
    private void MoveUpWrapped(bool resetAnchor)
    {
        var caret = Session.Caret;
        int visualLineIndex = WordWrapHelper.GetVisualLineIndexForPosition(
            Session.Document, _viewportWidth, caret.Line, caret.Column);

        if (visualLineIndex <= 0)
        {
            // Already at first visual line - move to start of document
            Session.Caret = new TextPosition(0, 0);
        }
        else
        {
            // Move to previous visual line, preserving column position if possible
            var newDocPos = GetDocumentPositionForVisualLine(visualLineIndex - 1, caret.Column);
            Session.Caret = newDocPos;
        }

        if (resetAnchor)
        {
            Session.Anchor = Session.Caret;
        }
    }

    private void MoveDownWrapped(bool resetAnchor)
    {
        var caret = Session.Caret;
        int visualLineIndex = WordWrapHelper.GetVisualLineIndexForPosition(
            Session.Document, _viewportWidth, caret.Line, caret.Column);
        int totalVisualLines = WordWrapHelper.GetTotalVisualLineCount(Session.Document, _viewportWidth);

        if (visualLineIndex >= totalVisualLines - 1)
        {
            // Already at last visual line - move to end of document
            int lastLine = Session.Document.Lines.Count - 1;
            Session.Caret = new TextPosition(lastLine, Session.GetLineLength(lastLine));
        }
        else
        {
            // Move to next visual line, preserving column position if possible
            var newDocPos = GetDocumentPositionForVisualLine(visualLineIndex + 1, caret.Column);
            Session.Caret = newDocPos;
        }

        if (resetAnchor)
        {
            Session.Anchor = Session.Caret;
        }
    }

    [ExcludeFromCodeCoverage]
    private TextPosition GetDocumentPositionForVisualLine(int targetVisualLine, int preferredColumn)
    {
        int visualLineIndex = 0;

        for (int docLine = 0; docLine < Session.Document.Lines.Count; docLine++)
        {
            var lineText = Session.Document.Lines[docLine].Content;
            var segments = WordWrapCalculator.WrapLine(lineText, _viewportWidth);

            for (int segIndex = 0; segIndex < segments.Count; segIndex++)
            {
                if (visualLineIndex == targetVisualLine)
                {
                    var segment = segments[segIndex];
                    // Clamp the preferred column to the segment bounds
                    int columnInSegment = Math.Min(preferredColumn, segment.Length);
                    return new TextPosition(docLine, segment.StartColumn + columnInSegment);
                }
                visualLineIndex++;
            }
        }

        // fallback to return end of document
        int lastLine = Session.Document.Lines.Count - 1;
        return new TextPosition(lastLine, Session.GetLineLength(lastLine));
    }
}
