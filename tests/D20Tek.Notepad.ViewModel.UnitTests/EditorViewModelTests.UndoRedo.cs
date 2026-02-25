using D20Tek.Notepad.Core;
using D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelUndoRedoTests
{
    private static readonly DocumentFactory _docFactory = new();

    // CanUndo/CanRedo Tests
    [TestMethod]
    public void CanUndo_WithNoChanges_ReturnsFalse()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);

        // act & assert
        Assert.IsFalse(viewModel.CanUndo);
    }

    [TestMethod]
    public void CanUndo_AfterTyping_ReturnsTrue()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(5);
        viewModel.TypeCharacter('!');

        // End the typing group so CanUndo reflects the pending operation
        viewModel.EndTypingGroupIfNeeded();

        // act & assert
        Assert.IsTrue(viewModel.CanUndo);
    }

    [TestMethod]
    public void CanRedo_WithNoUndoneChanges_ReturnsFalse()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);

        // act & assert
        Assert.IsFalse(viewModel.CanRedo);
    }

    [TestMethod]
    public void CanRedo_AfterUndo_ReturnsTrue()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(5);
        viewModel.TypeCharacter('!');
        viewModel.Undo();

        // act & assert
        Assert.IsTrue(viewModel.CanRedo);
    }

    // Undo Tests
    [TestMethod]
    public void Undo_RevertsLastChange()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);
        viewModel.TypeCharacter('!');
        Assert.AreEqual("Hello!", session.Document.Lines[0].Content);

        // act
        viewModel.Undo();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Undo_WithNoChanges_DoesNothing()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Undo();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Undo_MultipleChanges_RevertsInOrder()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // Type characters with navigation between them to create separate undo groups
        viewModel.TypeCharacter('!');
        viewModel.MoveRight(); // This ends the typing group and starts fresh
        viewModel.TypeCharacter('?');
        Assert.AreEqual("Hello!?", session.Document.Lines[0].Content);

        // act - undo should revert '?' first
        viewModel.Undo();

        // assert
        Assert.AreEqual("Hello!", session.Document.Lines[0].Content);

        // act - undo should revert '!'
        viewModel.Undo();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    // Redo Tests
    [TestMethod]
    public void Redo_ReappliesUndoneChange()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);
        viewModel.TypeCharacter('!');
        viewModel.Undo();
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);

        // act
        viewModel.Redo();

        // assert
        Assert.AreEqual("Hello!", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Redo_WithNoUndoneChanges_DoesNothing()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Redo();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Redo_AfterNewChange_ClearsRedoStack()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);
        viewModel.TypeCharacter('!');
        viewModel.Undo();
        Assert.IsTrue(viewModel.CanRedo);

        // act - type new character clears redo stack
        viewModel.TypeCharacter('?');
        viewModel.EndTypingGroupIfNeeded(); // End group so CanRedo reflects actual state

        // assert
        Assert.IsFalse(viewModel.CanRedo);
    }

    // Undo Grouping Tests
    [TestMethod]
    public void ContinuousTyping_GroupsMultipleChanges_IntoSingleUndo()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act - type multiple characters (automatic grouping)
        viewModel.TypeCharacter('!');
        viewModel.TypeCharacter('!');
        viewModel.TypeCharacter('!');

        Assert.AreEqual("Hello!!!", session.Document.Lines[0].Content);

        // Single undo should revert all grouped changes at once
        viewModel.Undo();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Undo_MarksDirty()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);
        viewModel.TypeCharacter('!');
        viewModel.ClearDirtyFlag();
        Assert.IsFalse(viewModel.IsDirty);

        // act
        viewModel.Undo();

        // assert
        Assert.IsTrue(viewModel.IsDirty);
    }

    [TestMethod]
    public void Redo_MarksDirty()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);
        viewModel.TypeCharacter('!');
        viewModel.Undo();
        viewModel.ClearDirtyFlag();
        Assert.IsFalse(viewModel.IsDirty);

        // act
        viewModel.Redo();

        // assert
        Assert.IsTrue(viewModel.IsDirty);
    }

    [TestMethod]
    public void Navigation_BreaksTypingGroup_CreatesSeparateUndos()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act - type, then navigate (breaks group), then type again
        viewModel.TypeCharacter('!');
        viewModel.TypeCharacter('!');
        viewModel.MoveLeft(); // Navigation breaks the typing group
        viewModel.MoveRight();
        viewModel.TypeCharacter('?');

        Assert.AreEqual("Hello!!?", session.Document.Lines[0].Content);

        // First undo should only revert '?'
        viewModel.Undo();
        Assert.AreEqual("Hello!!", session.Document.Lines[0].Content);

        // Second undo should revert '!!'
        viewModel.Undo();
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Backspace_BreaksTypingGroup()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act - type, then backspace (breaks group), then type again
        viewModel.TypeCharacter('!');
        viewModel.Backspace(); // Deletes '!' and breaks typing group
        viewModel.TypeCharacter('?');

        Assert.AreEqual("Hello?", session.Document.Lines[0].Content);

        // First undo should revert '?'
        viewModel.Undo();
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);

        // Second undo should revert the backspace (restore '!')
        viewModel.Undo();
        Assert.AreEqual("Hello!", session.Document.Lines[0].Content);

        // Third undo should revert the initial '!'
        viewModel.Undo();
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public async Task TypingGroupTimer_EndsGroupAfterTimeout()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act - type characters
        viewModel.TypeCharacter('!');
        viewModel.TypeCharacter('!');

        // Wait for timer to expire (2 seconds + buffer)
        await Task.Delay(2500);

        // Type more characters - should be in a new group
        viewModel.TypeCharacter('?');

        Assert.AreEqual("Hello!!?", session.Document.Lines[0].Content);

        // First undo should only revert '?' (new group after timer)
        viewModel.Undo();
        Assert.AreEqual("Hello!!", session.Document.Lines[0].Content);

        // Second undo should revert '!!' (original group)
        viewModel.Undo();
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);

        // Cleanup
        viewModel.DisposeTimers();
    }

    [TestMethod]
    public void DisposeTimers_CleansUpResources()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(5);
        viewModel.TypeCharacter('!'); // Starts timer

        // act - should not throw
        viewModel.DisposeTimers();
        viewModel.DisposeTimers(); // Should be safe to call multiple times

        // Typing after dispose should still work
        viewModel.TypeCharacter('?');

        // assert - typing still works after timer disposal
        viewModel.EndTypingGroupIfNeeded();
        Assert.IsTrue(viewModel.CanUndo);
    }

    private static (EditorViewModel viewModel, EditorSession session) CreateViewModel(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new DocumentData(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var viewModel = new EditorViewModel(session, new EditorCommandService(session));
        return (viewModel, session);
    }
}
