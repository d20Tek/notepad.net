namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel
{
    // Text Input Methods
    public void TypeCharacter(char c)
    {
        Commands.TypeCharacter(c);
        MarkDirty();
        Refresh();
        EnsureCaretVisible();
    }

    public void InsertText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        Commands.InsertText(text);
        MarkDirty();
        Refresh();
        EnsureCaretVisible();
    }

    public void InsertNewLine()
    {
        Commands.InsertNewLine();
        MarkDirty();
        Refresh();
        EnsureCaretVisible();
    }

    public void InsertTab()
    {
        // Insert tab character or spaces based on TabSize setting
        string tabContent = Settings.UseSpacesForTab
            ? new string(' ', Settings.TabSize)
            : "\t";
        Commands.InsertText(tabContent);
        MarkDirty();
        Refresh();
        EnsureCaretVisible();
    }

    // Deletion Methods
    public void Backspace()
    {
        Commands.Backspace();
        MarkDirty();
        Refresh();
        EnsureCaretVisible();
    }

    public void Delete()
    {
        Commands.Delete();
        MarkDirty();
        Refresh();
        EnsureCaretVisible();
    }

    public void DeleteSelection()
    {
        if (!Session.HasSelection) return;
        Commands.DeleteSelection();
        MarkDirty();
        Refresh();
        EnsureCaretVisible();
    }

    // Undo/Redo Methods
    public void Undo()
    {
        if (!CanUndo) return;
        Commands.Undo();
        MarkDirty();
        Refresh();
        EnsureCaretVisible();
    }

    public void Redo()
    {
        if (!CanRedo) return;
        Commands.Redo();
        MarkDirty();
        Refresh();
        EnsureCaretVisible();
    }

    public bool CanUndo => Session.UndoStack.CanUndo;

    public bool CanRedo => Session.UndoStack.CanRedo;

    public void BeginTypingGroup() => Commands.BeginTypingGroup();

    public void EndTypingGroup() => Commands.EndTypingGroup();

    // Selection Methods
    public void SelectAll()
    {
        Commands.SelectAll();
        Refresh();
    }
}
