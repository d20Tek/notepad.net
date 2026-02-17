using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.Core.Editing;

public sealed class EditorCommandService(EditorSession session, EditorNavigationService navigation)
{
    private readonly EditorSession _session = session ?? throw new ArgumentNullException(nameof(session));
    private readonly EditorNavigationService _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));

    // Typing & Editing Commands
    public void TypeCharacter(char c) => _session.InsertText(c.ToString());

    public void InsertText(string text) => _session.InsertText(text);

    public void InsertNewLine() => _session.InsertNewLine();

    public void Backspace() => _session.Backspace();

    public void Delete() => _session.Delete();

    public void ReplaceSelection(string text) => _session.ReplaceSelection(text);

    public void DeleteSelection() => _session.DeleteSelection();

    // Clipboard-like Commands
    public string CopySelection() => _session.GetSelectedText();

    public string CutSelection()
    {
        var text = _session.GetSelectedText();
        _session.DeleteSelection();
        return text;
    }

    public void Paste(string text) => _session.InsertText(text);

    // Navigation Commands
    public void MoveLeft() => _navigation.MoveLeft(_session);

    public void MoveRight() => _navigation.MoveRight(_session);
    
    public void MoveUp() => _navigation.MoveUp(_session);
    
    public void MoveDown() => _navigation.MoveDown(_session);

    public void MoveToLineStart() => _navigation.MoveToLineStart(_session);
    
    public void MoveToLineEnd() => _navigation.MoveToLineEnd(_session);

    public void MoveToDocumentStart() => _navigation.MoveToDocumentStart(_session);
    
    public void MoveToDocumentEnd() => _navigation.MoveToDocumentEnd(_session);

    // Selection Movement Commands
    public void SelectLeft() => _navigation.ExtendLeft(_session);

    public void SelectRight() => _navigation.ExtendRight(_session);

    public void SelectUp() => _navigation.ExtendUp(_session);

    public void SelectDown() => _navigation.ExtendDown(_session);

    public void SelectAll() => session.SelectAll();

    // Undo / Redo
    public void Undo() => _session.Undo();

    public void Redo() => _session.Redo();

    // Grouping (typing sessions)
    public void BeginTypingGroup() => _session.UndoStack.BeginGroup();

    public void EndTypingGroup() => _session.UndoStack.EndGroup();
}
