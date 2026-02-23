namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelWordWrapTests
{
    private static readonly DocumentFactory _docFactory = new();

    // IsWordWrapEnabled Tests
    [TestMethod]
    public void IsWordWrapEnabled_WithDefaultSettings_ReturnsFalse()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World"]);

        // act & assert
        Assert.IsFalse(viewModel.IsWordWrapEnabled);
    }

    [TestMethod]
    public void IsWordWrapEnabled_WithWordWrapSettings_ReturnsTrue()
    {
        // arrange
        var (viewModel, _) = CreateViewModelWithWordWrap(["Hello World"]);

        // act & assert
        Assert.IsTrue(viewModel.IsWordWrapEnabled);
    }

    // ToggleWordWrap Tests
    [TestMethod]
    public void ToggleWordWrap_WhenDisabled_EnablesWordWrap()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(80);

        // act
        viewModel.ToggleWordWrap();

        // assert
        Assert.IsTrue(viewModel.IsWordWrapEnabled);
        Assert.IsTrue(viewModel.Settings.WordWrapEnabled);
    }

    [TestMethod]
    public void ToggleWordWrap_WhenEnabled_DisablesWordWrap()
    {
        // arrange
        var (viewModel, _) = CreateViewModelWithWordWrap(["Hello World"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(80);

        // act
        viewModel.ToggleWordWrap();

        // assert
        Assert.IsFalse(viewModel.IsWordWrapEnabled);
        Assert.IsFalse(viewModel.Settings.WordWrapEnabled);
    }

    [TestMethod]
    public void ToggleWordWrap_WhenEnabling_ResetsHorizontalOffset()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World is a long line that extends beyond viewport"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(10);
        viewModel.ScrollColumns(5);
        Assert.AreEqual(5, viewModel.Viewport.HorizontalOffset);

        // act
        viewModel.ToggleWordWrap();

        // assert
        Assert.AreEqual(0, viewModel.Viewport.HorizontalOffset);
    }

    [TestMethod]
    public void ToggleWordWrap_FiresWordWrapChangedEvent()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(80);
        bool? eventValue = null;
        viewModel.WordWrapChanged += (enabled) => eventValue = enabled;

        // act
        viewModel.ToggleWordWrap();

        // assert
        Assert.IsTrue(eventValue);
    }

    [TestMethod]
    public void ToggleWordWrap_TriggersRefresh()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(80);
        bool viewChangedFired = false;
        viewModel.ViewChanged += () => viewChangedFired = true;

        // act
        viewModel.ToggleWordWrap();

        // assert
        Assert.IsTrue(viewChangedFired);
    }

    // SetWordWrap Tests
    [TestMethod]
    public void SetWordWrap_WithTrueWhenDisabled_EnablesWordWrap()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(80);

        // act
        viewModel.SetWordWrap(true);

        // assert
        Assert.IsTrue(viewModel.IsWordWrapEnabled);
    }

    [TestMethod]
    public void SetWordWrap_WithFalseWhenEnabled_DisablesWordWrap()
    {
        // arrange
        var (viewModel, _) = CreateViewModelWithWordWrap(["Hello World"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(80);

        // act
        viewModel.SetWordWrap(false);

        // assert
        Assert.IsFalse(viewModel.IsWordWrapEnabled);
    }

    [TestMethod]
    public void SetWordWrap_WithSameValue_DoesNotFireEvent()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(80);
        bool eventFired = false;
        viewModel.WordWrapChanged += (_) => eventFired = true;

        // act - already disabled, set to disabled
        viewModel.SetWordWrap(false);

        // assert
        Assert.IsFalse(eventFired);
    }

    [TestMethod]
    public void SetWordWrap_WithDifferentValue_FiresEvent()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(80);
        bool? eventValue = null;
        viewModel.WordWrapChanged += (enabled) => eventValue = enabled;

        // act
        viewModel.SetWordWrap(true);

        // assert
        Assert.IsTrue(eventValue);
    }

    [TestMethod]
    public void SetWordWrap_WhenEnabling_ResetsHorizontalOffset()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World is a long line"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(10);
        viewModel.ScrollColumns(5);

        // act
        viewModel.SetWordWrap(true);

        // assert
        Assert.AreEqual(0, viewModel.Viewport.HorizontalOffset);
    }

    // Word Wrap behavior with visible lines
    [TestMethod]
    public void ToggleWordWrap_UpdatesVisibleLinesForWrapping()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World Today"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(6);
        viewModel.Refresh();
        int linesBefore = viewModel.VisibleLines.Count;

        // act
        viewModel.ToggleWordWrap();

        // assert
        Assert.IsTrue(viewModel.VisibleLines.Count > linesBefore);
    }

    private static (EditorViewModel viewModel, EditorSession session) CreateViewModel(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new DocumentData(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var viewModel = new EditorViewModel(session, new EditorCommandService(session));
        return (viewModel, session);
    }

    private static (EditorViewModel viewModel, EditorSession session) CreateViewModelWithWordWrap(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new DocumentData(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var settings = new EditorSettings { WordWrapEnabled = true };
        var viewModel = new EditorViewModel(session, new EditorCommandService(session), settings);
        return (viewModel, session);
    }
}
