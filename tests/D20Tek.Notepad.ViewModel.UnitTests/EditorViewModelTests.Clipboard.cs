namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelClipboardTests
{
    private static readonly DocumentFactory _docFactory = new();

    // Cut Tests
    [TestMethod]
    public void Cut_WithSelection_CutsTextToClipboard()
    {
        // arrange
        var clipboard = new MockClipboardService();
        var (viewModel, session) = CreateViewModel(["Hello World"], clipboard);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 5);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Cut();

        // assert
        Assert.AreEqual("Hello", clipboard.GetText());
        Assert.AreEqual(" World", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Cut_WithoutSelection_DoesNothing()
    {
        // arrange
        var clipboard = new MockClipboardService();
        var (viewModel, session) = CreateViewModel(["Hello World"], clipboard);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Cut();

        // assert
        Assert.IsNull(clipboard.GetText());
        Assert.AreEqual("Hello World", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Cut_WithoutClipboardService_DoesNothing()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"], clipboardService: null);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 5);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Cut();

        // assert
        Assert.AreEqual("Hello World", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Cut_MarksDirty()
    {
        // arrange
        var clipboard = new MockClipboardService();
        var (viewModel, session) = CreateViewModel(["Hello World"], clipboard);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 5);
        viewModel.SetViewportHeight(5);
        Assert.IsFalse(viewModel.IsDirty);

        // act
        viewModel.Cut();

        // assert
        Assert.IsTrue(viewModel.IsDirty);
    }

    // Copy Tests
    [TestMethod]
    public void Copy_WithSelection_CopiesToClipboard()
    {
        // arrange
        var clipboard = new MockClipboardService();
        var (viewModel, session) = CreateViewModel(["Hello World"], clipboard);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 5);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Copy();

        // assert
        Assert.AreEqual("Hello", clipboard.GetText());
        Assert.AreEqual("Hello World", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Copy_WithoutSelection_DoesNothing()
    {
        // arrange
        var clipboard = new MockClipboardService();
        var (viewModel, session) = CreateViewModel(["Hello World"], clipboard);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Copy();

        // assert
        Assert.IsNull(clipboard.GetText());
    }

    [TestMethod]
    public void Copy_WithoutClipboardService_DoesNothing()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"], clipboardService: null);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 5);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Copy();

        // assert
        Assert.AreEqual("Hello World", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Copy_DoesNotMarkDirty()
    {
        // arrange
        var clipboard = new MockClipboardService();
        var (viewModel, session) = CreateViewModel(["Hello World"], clipboard);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 5);
        viewModel.SetViewportHeight(5);
        Assert.IsFalse(viewModel.IsDirty);

        // act
        viewModel.Copy();

        // assert
        Assert.IsFalse(viewModel.IsDirty);
    }

    // Paste Tests
    [TestMethod]
    public void Paste_WithClipboardContent_InsertsText()
    {
        // arrange
        var clipboard = new MockClipboardService();
        clipboard.SetText("World");
        var (viewModel, session) = CreateViewModel(["Hello "], clipboard);
        session.Caret = new TextPosition(0, 6);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Paste();

        // assert
        Assert.AreEqual("Hello World", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Paste_WithEmptyClipboard_DoesNothing()
    {
        // arrange
        var clipboard = new MockClipboardService();
        var (viewModel, session) = CreateViewModel(["Hello"], clipboard);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Paste();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Paste_WithoutClipboardService_DoesNothing()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"], clipboardService: null);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Paste();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Paste_ReplacesSelection_WhenSelectionActive()
    {
        // arrange
        var clipboard = new MockClipboardService();
        clipboard.SetText("Universe");
        var (viewModel, session) = CreateViewModel(["Hello World"], clipboard);
        session.Anchor = new TextPosition(0, 6);
        session.Caret = new TextPosition(0, 11);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Paste();

        // assert
        Assert.AreEqual("Hello Universe", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Paste_MarksDirty()
    {
        // arrange
        var clipboard = new MockClipboardService();
        clipboard.SetText("!");
        var (viewModel, session) = CreateViewModel(["Hello"], clipboard);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);
        Assert.IsFalse(viewModel.IsDirty);

        // act
        viewModel.Paste();

        // assert
        Assert.IsTrue(viewModel.IsDirty);
    }

    // CanCut Tests
    [TestMethod]
    public void CanCut_ReturnsTrue_WhenSelectionExists()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 5);

        // act
        var result = viewModel.CanCut;

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CanCut_ReturnsFalse_WhenNoSelection()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        var result = viewModel.CanCut;

        // assert
        Assert.IsFalse(result);
    }

    // CanCopy Tests
    [TestMethod]
    public void CanCopy_ReturnsTrue_WhenSelectionExists()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 5);

        // act
        var result = viewModel.CanCopy;

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CanCopy_ReturnsFalse_WhenNoSelection()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        var result = viewModel.CanCopy;

        // assert
        Assert.IsFalse(result);
    }

    // CanPaste Tests
    [TestMethod]
    public void CanPaste_ReturnsTrue_WhenClipboardHasText()
    {
        // arrange
        var clipboard = new MockClipboardService();
        clipboard.SetText("some text");
        var (viewModel, _) = CreateViewModel(["Hello"], clipboard);

        // act
        var result = viewModel.CanPaste;

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CanPaste_ReturnsFalse_WhenClipboardEmpty()
    {
        // arrange
        var clipboard = new MockClipboardService();
        var (viewModel, _) = CreateViewModel(["Hello"], clipboard);

        // act
        var result = viewModel.CanPaste;

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void CanPaste_ReturnsFalse_WhenNoClipboardService()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"], clipboardService: null);

        // act
        var result = viewModel.CanPaste;

        // assert
        Assert.IsFalse(result);
    }

    private static (EditorViewModel viewModel, EditorSession session) CreateViewModel(
        string[] lines,
        IClipboardService? clipboardService = null,
        EditorSettings? settings = null)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new DocumentData(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var viewModel = new EditorViewModel(
            session,
            new EditorCommandService(session),
            settings ?? EditorSettings.Default,
            clipboardService);
        return (viewModel, session);
    }
}
