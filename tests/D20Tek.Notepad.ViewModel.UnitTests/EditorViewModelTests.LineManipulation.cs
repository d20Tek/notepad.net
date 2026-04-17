using D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelLineManipulationTests
{
    private static readonly DocumentFactory _docFactory = new();

    // DuplicateLine Tests
    [TestMethod]
    public void DuplicateLine_DuplicatesCurrentLine()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.DuplicateLine();

        // assert
        Assert.AreEqual(3, session.Document.LineCount);
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
        Assert.AreEqual("Hello", session.Document.Lines[1].Content);
        Assert.IsTrue(viewModel.IsDirty);
    }

    [TestMethod]
    public void DuplicateLine_WithSelection_DuplicatesSelection()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World"]);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 5);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.DuplicateLine();

        // assert
        Assert.AreEqual(3, session.Document.LineCount);
        Assert.AreEqual("Hello", session.Document.Lines[1].Content);
        Assert.IsTrue(viewModel.IsDirty);
    }

    // MoveLineUp Tests
    [TestMethod]
    public void MoveLineUp_SwapsWithPreviousLine()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["First", "Second"]);
        session.Caret = new TextPosition(1, 0);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.MoveLineUp();

        // assert
        Assert.AreEqual("Second", session.Document.Lines[0].Content);
        Assert.AreEqual("First", session.Document.Lines[1].Content);
        Assert.IsTrue(viewModel.IsDirty);
    }

    [TestMethod]
    public void MoveLineUp_AtFirstLine_DoesNothing()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["First", "Second"]);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.MoveLineUp();

        // assert
        Assert.AreEqual("First", session.Document.Lines[0].Content);
        Assert.IsFalse(viewModel.IsDirty);
    }

    // MoveLineDown Tests
    [TestMethod]
    public void MoveLineDown_SwapsWithNextLine()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["First", "Second"]);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.MoveLineDown();

        // assert
        Assert.AreEqual("Second", session.Document.Lines[0].Content);
        Assert.AreEqual("First", session.Document.Lines[1].Content);
        Assert.IsTrue(viewModel.IsDirty);
    }

    [TestMethod]
    public void MoveLineDown_AtLastLine_DoesNothing()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["First", "Second"]);
        session.Caret = new TextPosition(1, 0);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.MoveLineDown();

        // assert
        Assert.AreEqual("First", session.Document.Lines[0].Content);
        Assert.IsFalse(viewModel.IsDirty);
    }

    // IndentSelection Tests
    [TestMethod]
    public void IndentSelection_IndentsCurrentLine()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.IndentSelection();

        // assert
        Assert.AreEqual("    Hello", session.Document.Lines[0].Content);
        Assert.IsTrue(viewModel.IsDirty);
    }

    [TestMethod]
    public void IndentSelection_WithMultiLineSelection_IndentsAllLines()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Line1", "Line2", "Line3"]);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(2, 3);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.IndentSelection();

        // assert
        Assert.AreEqual("    Line1", session.Document.Lines[0].Content);
        Assert.AreEqual("    Line2", session.Document.Lines[1].Content);
        Assert.AreEqual("    Line3", session.Document.Lines[2].Content);
    }

    [TestMethod]
    public void IndentSelection_WithTabSetting_UsesTab()
    {
        // arrange
        var settings = new EditorSettings { UseSpacesForTab = false };
        var (viewModel, session) = CreateViewModel(["Hello"], settings);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.IndentSelection();

        // assert
        Assert.AreEqual("\tHello", session.Document.Lines[0].Content);
    }

    // OutdentSelection Tests
    [TestMethod]
    public void OutdentSelection_RemovesIndent()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["    Hello"]);
        session.Caret = new TextPosition(0, 4);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.OutdentSelection();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
        Assert.IsTrue(viewModel.IsDirty);
    }

    [TestMethod]
    public void OutdentSelection_WithNoIndent_DoesNothing()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.OutdentSelection();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void OutdentSelection_WithTabSetting_RemovesTab()
    {
        // arrange
        var settings = new EditorSettings { UseSpacesForTab = false };
        var (viewModel, session) = CreateViewModel(["\tHello"], settings);
        session.Caret = new TextPosition(0, 1);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.OutdentSelection();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    // TrimTrailingWhitespace Tests
    [TestMethod]
    public void TrimTrailingWhitespace_RemovesTrailingSpaces()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello   ", "World  "]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.TrimTrailingWhitespace();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
        Assert.AreEqual("World", session.Document.Lines[1].Content);
        Assert.IsTrue(viewModel.IsDirty);
    }

    [TestMethod]
    public void TrimTrailingWhitespace_WithNoTrailing_DoesNothing()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.TrimTrailingWhitespace();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
        Assert.AreEqual("World", session.Document.Lines[1].Content);
    }

    private static (EditorViewModel viewModel, EditorSession session) CreateViewModel(
        string[] lines,
        EditorSettings? settings = null)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new DocumentData(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var viewModel = new EditorViewModel(
            session,
            new EditorCommandService(session),
            settings ?? EditorSettings.Default,
            new MockClipboardService());
        return (viewModel, session);
    }
}
