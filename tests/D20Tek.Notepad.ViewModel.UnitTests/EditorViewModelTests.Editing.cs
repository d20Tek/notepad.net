using D20Tek.Notepad.Core;
using D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelEditingTests
{
    private static readonly DocumentFactory _docFactory = new();

    // TypeCharacter Tests
    [TestMethod]
    public void TypeCharacter_InsertsCharacterAtCaret()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.TypeCharacter('!');

        // assert
        Assert.AreEqual("Hello!", session.Document.Lines[0].Content);
        Assert.AreEqual(6, session.Caret.Column);
    }

    [TestMethod]
    public void TypeCharacter_WithSelection_ReplacesSelection()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 5);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.TypeCharacter('X');

        // assert
        Assert.AreEqual("X World", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void TypeCharacter_MarksDirty()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(5);
        Assert.IsFalse(viewModel.IsDirty);

        // act
        viewModel.TypeCharacter('!');

        // assert
        Assert.IsTrue(viewModel.IsDirty);
    }

    // InsertText Tests
    [TestMethod]
    public void InsertText_InsertsTextAtCaret()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.InsertText(" World");

        // assert
        Assert.AreEqual("Hello World", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void InsertText_WithNullText_ThrowsArgumentNullException()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(5);

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => viewModel.InsertText(null!));
    }

    // InsertNewLine Tests
    [TestMethod]
    public void InsertNewLine_SplitsLine()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["HelloWorld"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.InsertNewLine();

        // assert
        Assert.AreEqual(2, session.Document.LineCount);
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
        Assert.AreEqual("World", session.Document.Lines[1].Content);
        Assert.AreEqual(1, session.Caret.Line);
        Assert.AreEqual(0, session.Caret.Column);
    }

    [TestMethod]
    public void InsertNewLine_MarksDirty()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.InsertNewLine();

        // assert
        Assert.IsTrue(viewModel.IsDirty);
    }

    // InsertTab Tests
    [TestMethod]
    public void InsertTab_WithUseSpacesForTabFalse_InsertsTabCharacter()
    {
        // arrange
        var settings = new EditorSettings { UseSpacesForTab = false };
        var (viewModel, session) = CreateViewModel(["Hello"], settings);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.InsertTab();

        // assert
        Assert.AreEqual("\tHello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void InsertTab_WithUseSpacesForTabTrue_InsertsSpaces()
    {
        // arrange
        var settings = new EditorSettings { UseSpacesForTab = true, TabSize = 4 };
        var (viewModel, session) = CreateViewModel(["Hello"], settings);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.InsertTab();

        // assert
        Assert.AreEqual("    Hello", session.Document.Lines[0].Content);
    }

    // Backspace Tests
    [TestMethod]
    public void Backspace_DeletesCharacterBeforeCaret()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Backspace();

        // assert
        Assert.AreEqual("Hell", session.Document.Lines[0].Content);
        Assert.AreEqual(4, session.Caret.Column);
    }

    [TestMethod]
    public void Backspace_AtLineStart_JoinsWithPreviousLine()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World"]);
        session.Caret = new TextPosition(1, 0);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Backspace();

        // assert
        Assert.AreEqual(1, session.Document.LineCount);
        Assert.AreEqual("HelloWorld", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Backspace_WithSelection_DeletesSelection()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 6);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Backspace();

        // assert
        Assert.AreEqual("World", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Backspace_MarksDirty()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Backspace();

        // assert
        Assert.IsTrue(viewModel.IsDirty);
    }

    // Delete Tests
    [TestMethod]
    public void Delete_DeletesCharacterAfterCaret()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Delete();

        // assert
        Assert.AreEqual("ello", session.Document.Lines[0].Content);
        Assert.AreEqual(0, session.Caret.Column);
    }

    [TestMethod]
    public void Delete_AtLineEnd_JoinsWithNextLine()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Delete();

        // assert
        Assert.AreEqual(1, session.Document.LineCount);
        Assert.AreEqual("HelloWorld", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Delete_MarksDirty()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Delete();

        // assert
        Assert.IsTrue(viewModel.IsDirty);
    }

    // DeleteSelection Tests
    [TestMethod]
    public void DeleteSelection_WithSelection_RemovesSelectedText()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 6);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.DeleteSelection();

        // assert
        Assert.AreEqual("World", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void DeleteSelection_WithoutSelection_DoesNothing()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.DeleteSelection();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    // SelectAll Tests
    [TestMethod]
    public void SelectAll_SelectsEntireDocument()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World", "Test"]);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.SelectAll();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Anchor);
        Assert.AreEqual(new TextPosition(2, 4), session.Caret); // End of "Test"
    }

    [TestMethod]
    public void SelectAll_WithSingleLine_SelectsLine()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.SelectAll();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Anchor);
        Assert.AreEqual(new TextPosition(0, 5), session.Caret);
    }

    private static (EditorViewModel viewModel, EditorSession session) CreateViewModel(
        string[] lines,
        EditorSettings? settings = null)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new DocumentData(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var viewModel = new EditorViewModel(session, new EditorCommandService(session), settings ?? EditorSettings.Default);
        return (viewModel, session);
    }
}
