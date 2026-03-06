namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelStatusTests
{
    private static readonly DocumentFactory _docFactory = new();

    [TestMethod]
    public void CurrentLine_ReturnsOneBasedLineNumber()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Line 1", "Line 2", "Line 3"]);
        session.Caret = new TextPosition(2, 0);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Refresh();

        // assert
        Assert.AreEqual(3, viewModel.CurrentStatus.CurrentLine);
    }

    [TestMethod]
    public void CurrentColumn_ReturnsOneBasedColumnNumber()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Refresh();

        // assert
        Assert.AreEqual(6, viewModel.CurrentStatus.CurrentColumn);
    }

    [TestMethod]
    public void CurrentLine_UpdatesOnCaretMove()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Line 1", "Line 2"]);
        viewModel.SetViewportHeight(5);
        viewModel.Refresh();

        // act
        session.Caret = new TextPosition(1, 0);
        session.Anchor = session.Caret;
        viewModel.Refresh();

        // assert
        Assert.AreEqual(2, viewModel.CurrentStatus.CurrentLine);
    }

    [TestMethod]
    public void CurrentColumn_UpdatesOnCaretMove()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        viewModel.SetViewportHeight(5);
        viewModel.Refresh();

        // act
        session.Caret = new TextPosition(0, 7);
        session.Anchor = session.Caret;
        viewModel.Refresh();

        // assert
        Assert.AreEqual(8, viewModel.CurrentStatus.CurrentColumn);
    }

    [TestMethod]
    public void TotalLines_ReturnsDocumentLineCount()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Line 1", "Line 2", "Line 3", "Line 4"]);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Refresh();

        // assert
        Assert.AreEqual(4, viewModel.CurrentStatus.TotalLines);
    }

    [TestMethod]
    public void TotalCharacters_ReturnsTotalCharacterCount()
    {
        // arrange - "Hello" (5) + "World" (5) + 1 line ending between them = 11
        var (viewModel, _) = CreateViewModel(["Hello", "World"]);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Refresh();

        // assert
        Assert.AreEqual(11, viewModel.CurrentStatus.TotalCharacters);
    }

    [TestMethod]
    public void DocumentEncoding_ReturnsEncodingName()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"], Encoding.UTF8, LineEndingStyle.CRLF);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Refresh();

        // assert
        Assert.AreEqual("UTF-8", viewModel.CurrentStatus.DocumentEncoding);
    }

    [TestMethod]
    public void LineEndingStyle_ReturnsCRLF()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"], Encoding.UTF8, LineEndingStyle.CRLF);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Refresh();

        // assert
        Assert.AreEqual("CRLF", viewModel.CurrentStatus.LineEndingStyle);
    }

    [TestMethod]
    public void LineEndingStyle_ReturnsLF()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"], Encoding.UTF8, LineEndingStyle.LF);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Refresh();

        // assert
        Assert.AreEqual("LF", viewModel.CurrentStatus.LineEndingStyle);
    }

    [TestMethod]
    public void LineEndingStyle_ReturnsCR()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"], Encoding.UTF8, LineEndingStyle.CR);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Refresh();

        // assert
        Assert.AreEqual("CR", viewModel.CurrentStatus.LineEndingStyle);
    }

    [TestMethod]
    public void StatusChanged_FiredOnCaretMove()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        viewModel.SetViewportHeight(5);
        viewModel.Refresh();

        StatusDetails? received = null;
        viewModel.StatusChanged += s => received = s;

        // act
        session.Caret = new TextPosition(0, 5);
        session.Anchor = session.Caret;
        viewModel.Refresh();

        // assert
        Assert.IsNotNull(received);
        Assert.AreEqual(6, received.CurrentColumn);
    }

    [TestMethod]
    public void StatusChanged_FiredOnDocumentChange()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(5);
        viewModel.Refresh();

        StatusDetails? received = null;
        viewModel.StatusChanged += s => received = s;

        // act
        viewModel.TypeCharacter('!');

        // assert
        Assert.IsNotNull(received);
        Assert.AreEqual(6, received.TotalCharacters);
    }

    private static (EditorViewModel viewModel, EditorSession session) CreateViewModel(
        string[] lines,
        Encoding? encoding = null,
        LineEndingStyle lineEnding = LineEndingStyle.CRLF)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new DocumentData(textLines, encoding ?? Encoding.UTF8, lineEnding));
        var session = new EditorSession(doc);
        var viewModel = new EditorViewModel(
            session,
            new EditorCommandService(session),
            EditorSettings.Default,
            new MockClipboardService());
        return (viewModel, session);
    }
}
