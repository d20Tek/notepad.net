namespace D20Tek.Notepad.Core.Editing;

public sealed class EditorCommandService(EditorSession session)
{
    private readonly EditorSession _session = session ?? throw new ArgumentNullException(nameof(session));

    // Typing & Editing Commands
    public void TypeCharacter(char c) => _session.InsertText(c.ToString());

    public void OverwriteCharacter(char c) => _session.OverwriteCharacter(c);

    public void InsertText(string text) => _session.InsertText(text);

    public void InsertNewLine() => _session.InsertNewLine();

    public void Backspace() => _session.Backspace();

    public void Delete() => _session.Delete();

    public void ReplaceSelection(string text) => _session.ReplaceSelection(text);

    public void DeleteSelection() => _session.DeleteSelection();

    public void DeleteWordLeft() => _session.DeleteWordLeft();

    public void DeleteWordRight() => _session.DeleteWordRight();

    // Clipboard-like Commands
    public string CopySelection() => _session.GetSelectedText();

    public string CutSelection()
    {
        var text = _session.GetSelectedText();
        _session.DeleteSelection();
        return text;
    }

    public void Paste(string text) => _session.InsertText(text);

    public void SelectAll() => _session.SelectAll();

    public void DuplicateLine() => _session.DuplicateLine();

    public void MoveLineUp() => _session.MoveLineUp();

    public void MoveLineDown() => _session.MoveLineDown();

    public void IndentLines(string indent) => _session.IndentLines(indent);

    public void OutdentLines(string indent) => _session.OutdentLines(indent);

    public void TrimTrailingWhitespace() => _session.TrimTrailingWhitespace();

    // Undo / Redo
    public void Undo() => _session.Undo();

    public void Redo() => _session.Redo();

    public void BeginTypingGroup() => _session.UndoStack.BeginGroup();

    public void EndTypingGroup() => _session.UndoStack.EndGroup();
}
