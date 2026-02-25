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
        viewModel.TypeCharacter('!');
        viewModel.TypeCharacter('?');
        Assert.AreEqual("Hello!?", session.Document.Lines[0].Content);

        // act
        viewModel.Undo();

        // assert
        Assert.AreEqual("Hello!", session.Document.Lines[0].Content);

        // act
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

        // assert
        Assert.IsFalse(viewModel.CanRedo);
    }

    // Undo Grouping Tests
    [TestMethod]
    public void BeginTypingGroup_GroupsMultipleChanges()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act - group multiple characters
        viewModel.BeginTypingGroup();
        viewModel.TypeCharacter('!');
        viewModel.TypeCharacter('!');
        viewModel.TypeCharacter('!');
        viewModel.EndTypingGroup();

        Assert.AreEqual("Hello!!!", session.Document.Lines[0].Content);

        // undo should revert all grouped changes at once
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

    private static (EditorViewModel viewModel, EditorSession session) CreateViewModel(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new DocumentData(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var viewModel = new EditorViewModel(session, new EditorCommandService(session));
        return (viewModel, session);
    }
}
