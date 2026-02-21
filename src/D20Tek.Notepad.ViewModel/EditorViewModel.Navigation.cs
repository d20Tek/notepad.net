namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel
{
    // Navigation (delegates to CaretNavigator with viewport awareness)
    public void MoveLeft() { Session.Navigator.MoveLeft(); EnsureCaretVisible(); }

    public void MoveRight() { Session.Navigator.MoveRight(); EnsureCaretVisible(); }
    
    public void MoveUp() { Session.Navigator.MoveUp(); EnsureCaretVisible(); }
    
    public void MoveDown() { Session.Navigator.MoveDown(); EnsureCaretVisible(); }
    
    public void MovePageUp() { Session.Navigator.MovePageUp(Viewport.VisibleLineCount); EnsureCaretVisible(); }
    
    public void MovePageDown() { Session.Navigator.MovePageDown(Viewport.VisibleLineCount); EnsureCaretVisible(); }
    
    public void MoveToLineStart() { Session.Navigator.MoveToLineStart(); EnsureCaretVisible(); }
    
    public void MoveToLineEnd() { Session.Navigator.MoveToLineEnd(); EnsureCaretVisible(); }
    
    public void MoveToDocumentStart() { Session.Navigator.MoveToDocumentStart(); EnsureCaretVisible(); }
    
    public void MoveToDocumentEnd() { Session.Navigator.MoveToDocumentEnd(); EnsureCaretVisible(); }

    // Selection Extension (delegates to CaretNavigator with viewport awareness)
    public void ExtendLeft() { Session.Navigator.ExtendLeft(); EnsureCaretVisible(); }
    
    public void ExtendRight() { Session.Navigator.ExtendRight(); EnsureCaretVisible(); }
    
    public void ExtendUp() { Session.Navigator.ExtendUp(); EnsureCaretVisible(); }
    
    public void ExtendDown() { Session.Navigator.ExtendDown(); EnsureCaretVisible(); }
    
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
}
