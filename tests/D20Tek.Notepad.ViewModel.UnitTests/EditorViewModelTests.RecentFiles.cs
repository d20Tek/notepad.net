namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelRecentFilesTests
{
    private static readonly DocumentFactory _docFactory = new();

    [TestMethod]
    public void RecentFiles_WhenSettingsEmpty_ReturnsEmptyList()
    {
        // arrange
        var viewModel = CreateViewModel();

        // act & assert
        Assert.IsEmpty(viewModel.RecentFiles);
    }

    [TestMethod]
    public void RecentFiles_WhenSettingsHasPaths_ReturnsPaths()
    {
        // arrange
        var paths = new List<string> { @"C:\files\a.txt", @"C:\files\b.txt" };
        var settings = EditorSettings.Default with { RecentFiles = paths };
        var viewModel = CreateViewModel(settings);

        // act & assert
        Assert.AreEqual(paths, viewModel.RecentFiles);
    }

    [TestMethod]
    public void SetFilePath_WithValidPath_AddsToRecentFiles()
    {
        // arrange
        var viewModel = CreateViewModel();

        // act
        viewModel.SetFilePath(@"C:\files\a.txt");

        // assert
        Assert.HasCount(1, viewModel.RecentFiles);
        Assert.AreEqual(@"C:\files\a.txt", viewModel.RecentFiles[0]);
    }

    [TestMethod]
    public void SetFilePath_WithSamePath_MovesPathToFront()
    {
        // arrange
        var viewModel = CreateViewModel();
        viewModel.SetFilePath(@"C:\files\a.txt");
        viewModel.SetFilePath(@"C:\files\b.txt");

        // act
        viewModel.SetFilePath(@"C:\files\a.txt");

        // assert
        Assert.AreEqual(@"C:\files\a.txt", viewModel.RecentFiles[0]);
        Assert.AreEqual(@"C:\files\b.txt", viewModel.RecentFiles[1]);
        Assert.HasCount(2, viewModel.RecentFiles);
    }

    [TestMethod]
    public void SetFilePath_WithNullPath_DoesNotUpdateRecentFiles()
    {
        // arrange
        var viewModel = CreateViewModel();
        viewModel.SetFilePath(@"C:\files\a.txt");

        // act
        viewModel.ClearFilePath();

        // assert
        Assert.HasCount(1, viewModel.RecentFiles);
    }

    [TestMethod]
    public void SetFilePath_FiresRecentFilesChangedEvent()
    {
        // arrange
        var viewModel = CreateViewModel();
        IReadOnlyList<string>? eventPaths = null;
        viewModel.RecentFilesChanged += paths => eventPaths = paths;

        // act
        viewModel.SetFilePath(@"C:\files\a.txt");

        // assert
        Assert.IsNotNull(eventPaths);
        Assert.HasCount(1, eventPaths);
        Assert.AreEqual(@"C:\files\a.txt", eventPaths[0]);
    }

    [TestMethod]
    public void SetFilePath_DoesNotFireRecentFilesChangedWhenPathNull()
    {
        // arrange
        var viewModel = CreateViewModel();
        int fireCount = 0;
        viewModel.RecentFilesChanged += [ExcludeFromCodeCoverage] (_) => fireCount++;

        // act
        viewModel.ClearFilePath();

        // assert
        Assert.AreEqual(0, fireCount);
    }

    private static EditorViewModel CreateViewModel(EditorSettings? settings = null)
    {
        var doc = _docFactory.Create(new([new TextLine("Hello")], Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        return new EditorViewModel(session, new EditorCommandService(session),
            settings ?? EditorSettings.Default, new MockClipboardService());
    }
}
