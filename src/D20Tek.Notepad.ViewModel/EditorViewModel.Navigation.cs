using D20Tek.Notepad.Core.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel
{
    // Navigation (delegates to CaretNavigator with viewport awareness)
    public void MoveLeft()
    {
        EndTypingGroupIfNeeded();
        Session.Navigator.MoveLeft();
        EnsureCaretVisible();
    }

    public void MoveRight()
    {
        EndTypingGroupIfNeeded(); 
        Session.Navigator.MoveRight(); 
        EnsureCaretVisible();
    }

    public void MoveUp()
    {
        EndTypingGroupIfNeeded();
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
        EndTypingGroupIfNeeded();
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

    public void MovePageUp()
    {
        EndTypingGroupIfNeeded();
        Session.Navigator.MovePageUp(Viewport.VisibleLineCount);
        EnsureCaretVisible();
    }

    public void MovePageDown()
    {
        EndTypingGroupIfNeeded();
        Session.Navigator.MovePageDown(Viewport.VisibleLineCount);
        EnsureCaretVisible();
    }

    public void MoveToLineStart()
    {
        EndTypingGroupIfNeeded();
        Session.Navigator.MoveToLineStart();
        EnsureCaretVisible();
    }

    public void MoveToLineEnd()
    {
        EndTypingGroupIfNeeded();
        Session.Navigator.MoveToLineEnd();
        EnsureCaretVisible();
    }

    public void MoveToDocumentStart()
    {
        EndTypingGroupIfNeeded();
        Session.Navigator.MoveToDocumentStart();
        EnsureCaretVisible();
    }

    public void MoveToDocumentEnd()
    {
        EndTypingGroupIfNeeded();
        Session.Navigator.MoveToDocumentEnd();
        EnsureCaretVisible();
    }

    // Selection Extension (delegates to CaretNavigator with viewport awareness)
    public void ExtendLeft()
    {
        EndTypingGroupIfNeeded();
        Session.Navigator.ExtendLeft();
        EnsureCaretVisible();
    }

    public void ExtendRight() 
    {
        EndTypingGroupIfNeeded();
        Session.Navigator.ExtendRight();
        EnsureCaretVisible();
    }

    public void ExtendUp()
    {
        EndTypingGroupIfNeeded();
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
        EndTypingGroupIfNeeded();
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

    public void ExtendPageUp()
    {
        EndTypingGroupIfNeeded();
        Session.Navigator.ExtendPageUp(Viewport.VisibleLineCount);
        EnsureCaretVisible();
    }

    public void ExtendPageDown()
    {
        EndTypingGroupIfNeeded();
        Session.Navigator.ExtendPageDown(Viewport.VisibleLineCount);
        EnsureCaretVisible();
    }

    public void ExtendToLineStart()
    {
        EndTypingGroupIfNeeded();
        Session.Navigator.ExtendToLineStart();
        EnsureCaretVisible();
    }

    public void ExtendToLineEnd()
    {
        EndTypingGroupIfNeeded();
        Session.Navigator.ExtendToLineEnd();
        EnsureCaretVisible();
    }

    public void ExtendToDocumentStart()
    {
        EndTypingGroupIfNeeded();
        Session.Navigator.ExtendToDocumentStart();
        EnsureCaretVisible();
    }

    public void ExtendToDocumentEnd()
    {
        EndTypingGroupIfNeeded();
        Session.Navigator.ExtendToDocumentEnd();
        EnsureCaretVisible();
    }

    // Word Navigation
    public void MoveWordLeft()
    {
        EndTypingGroupIfNeeded();
        Session.Navigator.MoveWordLeft();
        EnsureCaretVisible();
    }

    public void MoveWordRight()
    {
        EndTypingGroupIfNeeded();
        Session.Navigator.MoveWordRight();
        EnsureCaretVisible();
    }

    public void ExtendWordLeft()
    {
        EndTypingGroupIfNeeded();
        Session.Navigator.ExtendWordLeft();
        EnsureCaretVisible();
    }

    public void ExtendWordRight()
    {
        EndTypingGroupIfNeeded();
        Session.Navigator.ExtendWordRight();
        EnsureCaretVisible();
    }

    // Caret Positioning Helpers
    public int GetLineLength(int lineIndex) => Session.GetLineLength(lineIndex);

    public void SetAnchorToCaret() => Session.Anchor = Session.Caret;

    public void MoveCaretTo(int line, int column)
    {
        EndTypingGroupIfNeeded();
        var newPos = Session.ClampToDocument(line, column);
        Session.Caret = newPos;
        Session.Anchor = newPos;
        Refresh();
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
            var newDocPos = GetDocumentPositionForVisualLine(visualLineIndex - 1, GetVisualColumnForCaret(caret));
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
            var newDocPos = GetDocumentPositionForVisualLine(visualLineIndex + 1, GetVisualColumnForCaret(caret));
            Session.Caret = newDocPos;
        }

        if (resetAnchor)
        {
            Session.Anchor = Session.Caret;
        }
    }

    [ExcludeFromCodeCoverage]
    private int GetVisualColumnForCaret(TextPosition caret)
    {
        if (_viewportWidth <= 0 || caret.Line < 0 || caret.Line >= Session.Document.Lines.Count)
            return caret.Column;

        var segments = WordWrapCalculator.WrapLine(Session.Document.Lines[caret.Line].Content, _viewportWidth);

        for (int i = 0; i < segments.Count - 1; i++)
        {
            if (caret.Column <= segments[i].StartColumn + segments[i].Length)
                return caret.Column - segments[i].StartColumn;
        }

        return segments.Count > 0 ? caret.Column - segments[^1].StartColumn : caret.Column;
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
