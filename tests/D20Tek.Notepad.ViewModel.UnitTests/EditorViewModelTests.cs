namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelTests
{
    private static readonly DocumentFactory _docFactory = new();

    [TestMethod]
    public void Constructor_WithValidSession_SetsProperties()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);

        // act
        var viewModel = CreateViewModel(session);

        // assert
        Assert.AreSame(session, viewModel.Session);
        Assert.IsNotNull(viewModel.Viewport);
        Assert.IsNotNull(viewModel.VisibleLines);
        Assert.IsNotNull(viewModel.Commands);
        Assert.IsNotNull(viewModel.Settings);
    }

    [TestMethod]
    public void Constructor_WithValidSession_InitializesEmptyVisibleLines()
    {
        // arrange
        var session = CreateSession(["Hello"]);

        // act
        var viewModel = CreateViewModel(session);

        // assert
        Assert.IsEmpty(viewModel.VisibleLines);
    }

    // Refresh Tests
    [TestMethod]
    public void Refresh_UpdatesVisibleLines()
    {
        // arrange
        var session = CreateSession(["Line 1", "Line 2", "Line 3"]);
        var viewModel = CreateViewModel(session);
        viewModel.SetViewportHeight(2);

        // act
        viewModel.Refresh();

        // assert
        Assert.HasCount(2, viewModel.VisibleLines);
    }

    [TestMethod]
    public void Refresh_MapsCaretPosition()
    {
        // arrange
        var session = CreateSession(["Line 1", "Line 2", "Line 3"]);
        session.Caret = new TextPosition(1, 3);
        var viewModel = CreateViewModel(session);
        viewModel.SetViewportHeight(3);

        // act
        viewModel.Refresh();

        // assert
        Assert.AreEqual(new ViewPosition(1, 3), viewModel.CaretViewPosition);
    }

    [TestMethod]
    public void Refresh_WithNoSelection_SelectionViewRangeIsNull()
    {
        // arrange
        var session = CreateSession(["Line 1", "Line 2"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);
        var viewModel = CreateViewModel(session);
        viewModel.SetViewportHeight(2);

        // act
        viewModel.Refresh();

        // assert
        Assert.IsNull(viewModel.SelectionViewRange);
    }

    [TestMethod]
    public void Refresh_WithSelection_MapsSelectionViewRange()
    {
        // arrange
        var session = CreateSession(["Line 1", "Line 2", "Line 3"]);
        session.Anchor = new TextPosition(0, 2);
        session.Caret = new TextPosition(1, 4);
        var viewModel = CreateViewModel(session);
        viewModel.SetViewportHeight(3);

        // act
        viewModel.Refresh();

        // assert
        Assert.IsNotNull(viewModel.SelectionViewRange);
        Assert.AreEqual(new ViewPosition(0, 2), viewModel.SelectionViewRange.Value.Start);
        Assert.AreEqual(new ViewPosition(1, 4), viewModel.SelectionViewRange.Value.End);
    }

    // CaretViewPosition Tests
    [TestMethod]
    public void CaretViewPosition_AfterScroll_MapsToViewCoordinates()
    {
        // arrange
        var session = CreateSession(GenerateLines(10));
        session.Caret = new TextPosition(5, 3);
        var viewModel = CreateViewModel(session);
        viewModel.SetViewportHeight(5);
        viewModel.ScrollLines(3);

        // act
        viewModel.Refresh();

        // assert
        Assert.AreEqual(new ViewPosition(2, 3), viewModel.CaretViewPosition);
    }

    private static EditorSession CreateSession(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        return new EditorSession(doc);
    }

    private static EditorViewModel CreateViewModel(EditorSession session) =>
        new(session, new EditorCommandService(session));

    private static string[] GenerateLines(int count) => [.. Enumerable.Range(1, count).Select(i => $"Line {i}")];
}
