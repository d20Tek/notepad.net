namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class NavigatorSelectionTests
{
    private static readonly DocumentFactory _docFactory = new();

    // ExtendSelectionLeft tests
    [TestMethod]
    public void ExtendSelectionLeft_ExtendsSelectionLeftByOneColumn()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);
        viewModel.SetViewportHeight(1);

        // act
        viewModel.Navigator.ExtendSelectionLeft();

        // assert
        Assert.AreEqual(new TextPosition(0, 4), session.Caret);
        Assert.AreEqual(new TextPosition(0, 5), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    [TestMethod]
    public void ExtendSelectionLeft_ExtendingExistingSelection_KeepsAnchor()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Anchor = new TextPosition(0, 8);
        session.Caret = new TextPosition(0, 5);
        viewModel.SetViewportHeight(1);

        // act
        viewModel.Navigator.ExtendSelectionLeft();

        // assert
        Assert.AreEqual(new TextPosition(0, 4), session.Caret);
        Assert.AreEqual(new TextPosition(0, 8), session.Anchor);
    }

    // ExtendSelectionRight tests
    [TestMethod]
    public void ExtendSelectionRight_ExtendsSelectionRightByOneColumn()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);
        viewModel.SetViewportHeight(1);

        // act
        viewModel.Navigator.ExtendSelectionRight();

        // assert
        Assert.AreEqual(new TextPosition(0, 6), session.Caret);
        Assert.AreEqual(new TextPosition(0, 5), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    // ExtendSelectionUp tests
    [TestMethod]
    public void ExtendSelectionUp_ExtendsSelectionUpByOneLine()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World"]);
        session.Caret = new TextPosition(1, 3);
        session.Anchor = new TextPosition(1, 3);
        viewModel.SetViewportHeight(2);

        // act
        viewModel.Navigator.ExtendSelectionUp();

        // assert
        Assert.AreEqual(new TextPosition(0, 3), session.Caret);
        Assert.AreEqual(new TextPosition(1, 3), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    // ExtendSelectionDown tests
    [TestMethod]
    public void ExtendSelectionDown_ExtendsSelectionDownByOneLine()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);
        viewModel.SetViewportHeight(2);

        // act
        viewModel.Navigator.ExtendSelectionDown();

        // assert
        Assert.AreEqual(new TextPosition(1, 3), session.Caret);
        Assert.AreEqual(new TextPosition(0, 3), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    // ExtendSelectionToLineStart tests
    [TestMethod]
    public void ExtendSelectionToLineStart_ExtendsSelectionToColumnZero()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 6);
        session.Anchor = new TextPosition(0, 6);
        viewModel.SetViewportHeight(1);

        // act
        viewModel.Navigator.ExtendSelectionToLineStart();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
        Assert.AreEqual(new TextPosition(0, 6), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    [TestMethod]
    public void ExtendSelectionToLineStart_WithExistingSelection_KeepsOriginalAnchor()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Anchor = new TextPosition(0, 2);
        session.Caret = new TextPosition(0, 8);
        viewModel.SetViewportHeight(1);

        // act
        viewModel.Navigator.ExtendSelectionToLineStart();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
        Assert.AreEqual(new TextPosition(0, 2), session.Anchor);
    }

    // ExtendSelectionToLineEnd tests
    [TestMethod]
    public void ExtendSelectionToLineEnd_ExtendsSelectionToEndOfLine()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);
        viewModel.SetViewportHeight(1);

        // act
        viewModel.Navigator.ExtendSelectionToLineEnd();

        // assert
        Assert.AreEqual(new TextPosition(0, 11), session.Caret);
        Assert.AreEqual(new TextPosition(0, 3), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    [TestMethod]
    public void ExtendSelectionToLineEnd_WithExistingSelection_KeepsOriginalAnchor()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Anchor = new TextPosition(0, 6);
        session.Caret = new TextPosition(0, 3);
        viewModel.SetViewportHeight(1);

        // act
        viewModel.Navigator.ExtendSelectionToLineEnd();

        // assert
        Assert.AreEqual(new TextPosition(0, 11), session.Caret);
        Assert.AreEqual(new TextPosition(0, 6), session.Anchor);
    }

    // ExtendSelectionPageUp tests
    [TestMethod]
    public void ExtendSelectionPageUp_ExtendsSelectionUpByVisibleLineCount()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(GenerateLines(20));
        session.Caret = new TextPosition(15, 3);
        session.Anchor = new TextPosition(15, 3);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Navigator.ExtendSelectionPageUp();

        // assert
        Assert.AreEqual(new TextPosition(10, 3), session.Caret);
        Assert.AreEqual(new TextPosition(15, 3), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    // ExtendSelectionPageDown tests
    [TestMethod]
    public void ExtendSelectionPageDown_ExtendsSelectionDownByVisibleLineCount()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(GenerateLines(20));
        session.Caret = new TextPosition(5, 3);
        session.Anchor = new TextPosition(5, 3);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Navigator.ExtendSelectionPageDown();

        // assert
        Assert.AreEqual(new TextPosition(10, 3), session.Caret);
        Assert.AreEqual(new TextPosition(5, 3), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    // ExtendSelectionToDocumentStart tests
    [TestMethod]
    public void ExtendSelectionToDocumentStart_ExtendsSelectionToDocumentEnd()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World", "Test"]);
        session.Caret = new TextPosition(1, 2);
        session.Anchor = new TextPosition(1, 2);
        viewModel.SetViewportHeight(3);

        // act
        viewModel.Navigator.ExtendSelectionToDocumentStart();

        // assert
        Assert.AreEqual(new TextPosition(2, 4), session.Caret);
        Assert.AreEqual(new TextPosition(1, 2), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    // ExtendSelectionToDocumentEnd tests
    [TestMethod]
    public void ExtendSelectionToDocumentEnd_ExtendsSelectionToDocumentEnd()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World", "Test"]);
        session.Caret = new TextPosition(1, 2);
        session.Anchor = new TextPosition(1, 2);
        viewModel.SetViewportHeight(3);

        // act
        viewModel.Navigator.ExtendSelectionToDocumentEnd();

        // assert
        Assert.AreEqual(new TextPosition(2, 4), session.Caret);
        Assert.AreEqual(new TextPosition(1, 2), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    // EnsureCaretVisible tests
    [TestMethod]
    public void ExtendSelectionLeft_EnsuresCaretVisible()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World this is long"]);
        session.Caret = new TextPosition(0, 15);
        session.Anchor = new TextPosition(0, 15);
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(10);
        viewModel.ScrollColumns(12);

        // act
        viewModel.Navigator.ExtendSelectionLeft();
        viewModel.Navigator.ExtendSelectionLeft();

        // assert
        Assert.IsTrue(viewModel.Viewport.HorizontalOffset <= session.Caret.Column);
    }

    // Helper methods
    private static (EditorViewModel viewModel, EditorSession session) CreateViewModel(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var navigation = new EditorNavigationService();
        var commandService = new EditorCommandService(session, navigation);
        var viewModel = new EditorViewModel(session, commandService);
        return (viewModel, session);
    }

    private static string[] GenerateLines(int count) =>
        [.. Enumerable.Range(1, count).Select(i => $"Line {i}")];
}