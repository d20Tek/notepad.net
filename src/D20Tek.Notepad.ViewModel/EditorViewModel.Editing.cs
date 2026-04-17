namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel
{
    private bool _isInTypingGroup;
    private Timer? _typingGroupTimer;
    private static readonly TimeSpan TypingGroupTimeout = TimeSpan.FromSeconds(2);

    // Text Input Methods
    public void TypeCharacter(char c)
    {
        BeginTypingGroupIfNeeded();
        ResetTypingGroupTimer();

        bool atLineEnd = Session.Caret.Column >= Session.GetLineLength(Session.Caret.Line);

        if (_isOverwriteMode && !Session.HasSelection && !atLineEnd)
            Commands.OverwriteCharacter(c);
        else
            Commands.TypeCharacter(c);

        CheckDirtyStateChanged();
        EnsureCaretVisible();
    }

    public void InsertText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        EndTypingGroupIfNeeded();
        Commands.InsertText(text);
        CheckDirtyStateChanged();
        EnsureCaretVisible();
    }

    public void InsertNewLine()
    {
        EndTypingGroupIfNeeded();
        Commands.InsertNewLine();
        CheckDirtyStateChanged();
        EnsureCaretVisible();
    }

    public void InsertTab()
    {
        EndTypingGroupIfNeeded();
        // Insert tab character or spaces based on TabSize setting
        string tabContent = Settings.UseSpacesForTab
            ? new string(' ', Settings.TabSize)
            : "\t";
        Commands.InsertText(tabContent);
        CheckDirtyStateChanged();
        EnsureCaretVisible();
    }

    // Deletion Methods
    public void Backspace()
    {
        EndTypingGroupIfNeeded();
        Commands.Backspace();
        CheckDirtyStateChanged();
        EnsureCaretVisible();
    }

    public void Delete()
    {
        EndTypingGroupIfNeeded();
        Commands.Delete();
        CheckDirtyStateChanged();
        EnsureCaretVisible();
    }

    public void DeleteWordLeft()
    {
        EndTypingGroupIfNeeded();
        Commands.DeleteWordLeft();
        CheckDirtyStateChanged();
        EnsureCaretVisible();
    }

    public void DeleteWordRight()
    {
        EndTypingGroupIfNeeded();
        Commands.DeleteWordRight();
        CheckDirtyStateChanged();
        EnsureCaretVisible();
    }

    public void DeleteSelection()
    {
        if (!Session.HasSelection) return;
        EndTypingGroupIfNeeded();
        Commands.DeleteSelection();
        CheckDirtyStateChanged();
        EnsureCaretVisible();
    }

    // Undo/Redo Methods
    public void Undo()
    {
        EndTypingGroupIfNeeded();
        if (!CanUndo) return;
        Commands.Undo();
        CheckDirtyStateChanged();
        EnsureCaretVisible();
    }

    public void Redo()
    {
        EndTypingGroupIfNeeded();
        if (!CanRedo) return;
        Commands.Redo();
        CheckDirtyStateChanged();
        EnsureCaretVisible();
    }

    public bool CanUndo => Session.UndoStack.CanUndo;

    public bool CanRedo => Session.UndoStack.CanRedo;

    // Selection Methods
    public void SelectAll()
    {
        EndTypingGroupIfNeeded();
        Commands.SelectAll();
        Refresh();
    }

    // Line Manipulation Methods
    public void DuplicateLine()
    {
        EndTypingGroupIfNeeded();
        Commands.DuplicateLine();
        CheckDirtyStateChanged();
        EnsureCaretVisible();
    }

    public void MoveLineUp()
    {
        EndTypingGroupIfNeeded();
        Commands.MoveLineUp();
        CheckDirtyStateChanged();
        EnsureCaretVisible();
    }

    public void MoveLineDown()
    {
        EndTypingGroupIfNeeded();
        Commands.MoveLineDown();
        CheckDirtyStateChanged();
        EnsureCaretVisible();
    }

    public void IndentSelection()
    {
        EndTypingGroupIfNeeded();
        string indent = Settings.UseSpacesForTab
            ? new string(' ', Settings.TabSize)
            : "\t";
        Commands.IndentLines(indent);
        CheckDirtyStateChanged();
        EnsureCaretVisible();
    }

    public void OutdentSelection()
    {
        EndTypingGroupIfNeeded();
        string indent = Settings.UseSpacesForTab
            ? new string(' ', Settings.TabSize)
            : "\t";
        Commands.OutdentLines(indent);
        CheckDirtyStateChanged();
        EnsureCaretVisible();
    }

    public void TrimTrailingWhitespace()
    {
        EndTypingGroupIfNeeded();
        Commands.TrimTrailingWhitespace();
        CheckDirtyStateChanged();
        EnsureCaretVisible();
    }

    // Typing Group Management
    private void BeginTypingGroupIfNeeded()
    {
        if (_isInTypingGroup) return;

        Commands.BeginTypingGroup();
        _isInTypingGroup = true;
    }

    public void EndTypingGroupIfNeeded()
    {
        StopTypingGroupTimer();

        if (!_isInTypingGroup) return;

        Commands.EndTypingGroup();
        _isInTypingGroup = false;
    }

    private void ResetTypingGroupTimer()
    {
        // Dispose existing timer if any
        _typingGroupTimer?.Dispose();

        // Create new timer that fires once after the timeout
        _typingGroupTimer = new Timer(
            OnTypingGroupTimerElapsed,
            null,
            TypingGroupTimeout,
            Timeout.InfiniteTimeSpan); // Don't repeat
    }

    private void StopTypingGroupTimer()
    {
        _typingGroupTimer?.Dispose();
        _typingGroupTimer = null;
    }

    private void OnTypingGroupTimerElapsed(object? state)
    {
        // Timer fires on thread pool thread - need to marshal to UI thread
        // The actual EndTypingGroupIfNeeded call is thread-safe for the ViewModel state
        // but any UI refresh should be handled by the UI layer
        EndTypingGroupIfNeeded();
    }

    public void DisposeTimers()
    {
        StopTypingGroupTimer();
    }
}
