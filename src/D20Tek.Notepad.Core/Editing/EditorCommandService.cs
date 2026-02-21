namespace D20Tek.Notepad.Core.Editing;

/// <summary>
/// Service for editing commands (typing, clipboard, undo/redo).
/// Navigation is handled directly by EditorSession.Navigator (CaretNavigator).
/// </summary>
public sealed class EditorCommandService(EditorSession session)
{
    private readonly EditorSession _session = session ?? throw new ArgumentNullException(nameof(session));

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

    // Selection
    public void SelectAll() => _session.SelectAll();

    // Undo / Redo
    public void Undo() => _session.Undo();

    public void Redo() => _session.Redo();

    // Grouping (typing sessions)
    public void BeginTypingGroup() => _session.UndoStack.BeginGroup();

    public void EndTypingGroup() => _session.UndoStack.EndGroup();
}
