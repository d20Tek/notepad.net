namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelEventsTests
{
    private static readonly DocumentFactory _docFactory = new();

    // ViewChanged event tests
    [TestMethod]
    public void Refresh_AlwaysFiresViewChangedEvent()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello", "World"]);
        viewModel.SetViewportHeight(2);
        var viewChangedFired = false;
        viewModel.ViewChanged += () => viewChangedFired = true;

        // act
        viewModel.Refresh();

        // assert
        Assert.IsTrue(viewChangedFired);
    }

    [TestMethod]
    public void Refresh_FiresViewChangedEventOnEachCall()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello", "World"]);
        viewModel.SetViewportHeight(2);
        var viewChangedCount = 0;
        viewModel.ViewChanged += () => viewChangedCount++;

        // act
        viewModel.Refresh();
        viewModel.Refresh();
        viewModel.Refresh();

        // assert
        Assert.AreEqual(3, viewChangedCount);
    }

    // CaretMoved event tests
    [TestMethod]
    public void Refresh_WhenCaretPositionChanges_FiresCaretMovedEvent()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World"]);
        viewModel.SetViewportHeight(2);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);
        viewModel.Refresh(); // Initial refresh to set baseline

        ViewPosition? receivedPosition = null;
        viewModel.CaretMoved += pos => receivedPosition = pos;

        // act
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);
        viewModel.Refresh();

        // assert
        Assert.IsNotNull(receivedPosition);
        Assert.AreEqual(new ViewPosition(0, 5), receivedPosition.Value);
    }

    [TestMethod]
    public void Refresh_WhenCaretPositionUnchanged_DoesNotFireCaretMovedEvent()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World"]);
        viewModel.SetViewportHeight(2);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);
        viewModel.Refresh(); // Initial refresh to set baseline

        var caretMovedFired = false;
        viewModel.CaretMoved += [ExcludeFromCodeCoverage](_) => caretMovedFired = true;

        // act
        viewModel.Refresh(); // Refresh without changing caret

        // assert
        Assert.IsFalse(caretMovedFired);
    }

    [TestMethod]
    public void Refresh_WhenCaretLineChanges_FiresCaretMovedEvent()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World"]);
        viewModel.SetViewportHeight(2);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);
        viewModel.Refresh();

        ViewPosition? receivedPosition = null;
        viewModel.CaretMoved += pos => receivedPosition = pos;

        // act
        session.Caret = new TextPosition(1, 3);
        session.Anchor = new TextPosition(1, 3);
        viewModel.Refresh();

        // assert
        Assert.IsNotNull(receivedPosition);
        Assert.AreEqual(new ViewPosition(1, 3), receivedPosition.Value);
    }

    // SelectionChanged event tests
    [TestMethod]
    public void Refresh_WhenSelectionCreated_FiresSelectionChangedEvent()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        viewModel.SetViewportHeight(1);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);
        viewModel.Refresh(); // No selection initially

        SelectionViewRange? receivedSelection = null;
        var eventFired = false;
        viewModel.SelectionChanged += sel =>
        {
            eventFired = true;
            receivedSelection = sel;
        };

        // act
        session.Anchor = new TextPosition(0, 2);
        session.Caret = new TextPosition(0, 7);
        viewModel.Refresh();

        // assert
        Assert.IsTrue(eventFired);
        Assert.IsNotNull(receivedSelection);
    }

    [TestMethod]
    public void Refresh_WhenSelectionCleared_FiresSelectionChangedEvent()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        viewModel.SetViewportHeight(1);
        session.Anchor = new TextPosition(0, 2);
        session.Caret = new TextPosition(0, 7);
        viewModel.Refresh(); // Selection exists

        SelectionViewRange? receivedSelection = null;
        var eventFired = false;
        viewModel.SelectionChanged += sel =>
        {
            eventFired = true;
            receivedSelection = sel;
        };

        // act
        session.Anchor = new TextPosition(0, 5);
        session.Caret = new TextPosition(0, 5);
        viewModel.Refresh();

        // assert
        Assert.IsTrue(eventFired);
        Assert.IsNull(receivedSelection);
    }

    [TestMethod]
    public void Refresh_WhenSelectionChanges_FiresSelectionChangedEvent()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        viewModel.SetViewportHeight(1);
        session.Anchor = new TextPosition(0, 2);
        session.Caret = new TextPosition(0, 5);
        viewModel.Refresh();

        SelectionViewRange? receivedSelection = null;
        viewModel.SelectionChanged += sel => receivedSelection = sel;

        // act
        session.Caret = new TextPosition(0, 8); // Extend selection
        viewModel.Refresh();

        // assert
        Assert.IsNotNull(receivedSelection);
        Assert.AreEqual(8, receivedSelection.Value.End.Column);
    }

    [TestMethod]
    public void Refresh_WhenSelectionUnchanged_DoesNotFireSelectionChangedEvent()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        viewModel.SetViewportHeight(1);
        session.Anchor = new TextPosition(0, 2);
        session.Caret = new TextPosition(0, 7);
        viewModel.Refresh();

        var selectionChangedFired = false;
        viewModel.SelectionChanged += [ExcludeFromCodeCoverage](_) => selectionChangedFired = true;

        // act
        viewModel.Refresh(); // No change to selection

        // assert
        Assert.IsFalse(selectionChangedFired);
    }

    [TestMethod]
    public void Refresh_WhenNoSelectionAndStillNoSelection_DoesNotFireSelectionChangedEvent()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        viewModel.SetViewportHeight(1);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);
        viewModel.Refresh();

        var selectionChangedFired = false;
        viewModel.SelectionChanged += [ExcludeFromCodeCoverage](_) => selectionChangedFired = true;

        // act
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);
        viewModel.Refresh();

        // assert
        Assert.IsFalse(selectionChangedFired);
    }

    // Multiple events tests
    [TestMethod]
    public void Refresh_WhenBothCaretAndSelectionChange_FiresBothEvents()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        viewModel.SetViewportHeight(1);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);
        viewModel.Refresh();

        var caretMovedFired = false;
        var selectionChangedFired = false;
        viewModel.CaretMoved += _ => caretMovedFired = true;
        viewModel.SelectionChanged += _ => selectionChangedFired = true;

        // act
        session.Anchor = new TextPosition(0, 2);
        session.Caret = new TextPosition(0, 7);
        viewModel.Refresh();

        // assert
        Assert.IsTrue(caretMovedFired);
        Assert.IsTrue(selectionChangedFired);
    }

    [TestMethod]
    public void Refresh_WithNoEventHandlers_DoesNotThrow()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        viewModel.SetViewportHeight(1);

        // act & assert - should not throw
        session.Caret = new TextPosition(0, 5);
        viewModel.Refresh();
    }

    // Helper methods
    private static (EditorViewModel viewModel, EditorSession session) CreateViewModel(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var commandService = new EditorCommandService(session);
        var viewModel = new EditorViewModel(session, commandService, EditorSettings.Default, new MockClipboardService());
        return (viewModel, session);
    }
}