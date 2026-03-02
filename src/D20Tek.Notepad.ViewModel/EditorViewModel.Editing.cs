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
        Commands.TypeCharacter(c);
        CheckDirtyStateChanged();
        Refresh();
        EnsureCaretVisible();
    }

    public void InsertText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        EndTypingGroupIfNeeded();
        Commands.InsertText(text);
        CheckDirtyStateChanged();
        Refresh();
        EnsureCaretVisible();
    }

    public void InsertNewLine()
    {
        EndTypingGroupIfNeeded();
        Commands.InsertNewLine();
        CheckDirtyStateChanged();
        Refresh();
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
        Refresh();
        EnsureCaretVisible();
    }

    // Deletion Methods
    public void Backspace()
    {
        EndTypingGroupIfNeeded();
        Commands.Backspace();
        CheckDirtyStateChanged();
        Refresh();
        EnsureCaretVisible();
    }

    public void Delete()
    {
        EndTypingGroupIfNeeded();
        Commands.Delete();
        CheckDirtyStateChanged();
        Refresh();
        EnsureCaretVisible();
    }

    public void DeleteWordLeft()
    {
        EndTypingGroupIfNeeded();
        Commands.DeleteWordLeft();
        CheckDirtyStateChanged();
        Refresh();
        EnsureCaretVisible();
    }

    public void DeleteWordRight()
    {
        EndTypingGroupIfNeeded();
        Commands.DeleteWordRight();
        CheckDirtyStateChanged();
        Refresh();
        EnsureCaretVisible();
    }

    public void DeleteSelection()
    {
        if (!Session.HasSelection) return;
        EndTypingGroupIfNeeded();
        Commands.DeleteSelection();
        CheckDirtyStateChanged();
        Refresh();
        EnsureCaretVisible();
    }

    // Undo/Redo Methods
    public void Undo()
    {
        EndTypingGroupIfNeeded();
        if (!CanUndo) return;
        Commands.Undo();
        CheckDirtyStateChanged();
        Refresh();
        EnsureCaretVisible();
    }

    public void Redo()
    {
        EndTypingGroupIfNeeded();
        if (!CanRedo) return;
        Commands.Redo();
        CheckDirtyStateChanged();
        Refresh();
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
