namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelEncodingTests
{
    private static readonly DocumentFactory _docFactory = new();

    [TestMethod]
    public void ChangeEncoding_Utf8_SetsDocumentEncoding()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"], Encoding.Unicode);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.ChangeEncoding(new UTF8Encoding(false));

        // assert
        Assert.IsTrue(viewModel.IsCurrentEncoding(new UTF8Encoding(false)));
    }

    [TestMethod]
    public void ChangeEncoding_Utf8Bom_SetsDocumentEncoding()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"], new UTF8Encoding(false));
        viewModel.SetViewportHeight(5);

        // act
        viewModel.ChangeEncoding(new UTF8Encoding(true));

        // assert
        Assert.IsTrue(viewModel.IsCurrentEncoding(new UTF8Encoding(true)));
    }

    [TestMethod]
    public void ChangeEncoding_Unicode_SetsDocumentEncoding()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"], new UTF8Encoding(false));
        viewModel.SetViewportHeight(5);

        // act
        viewModel.ChangeEncoding(Encoding.Unicode);

        // assert
        Assert.IsTrue(viewModel.IsCurrentEncoding(Encoding.Unicode));
    }

    [TestMethod]
    public void ChangeEncoding_BigEndianUnicode_SetsDocumentEncoding()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"], new UTF8Encoding(false));
        viewModel.SetViewportHeight(5);

        // act
        viewModel.ChangeEncoding(Encoding.BigEndianUnicode);

        // assert
        Assert.IsTrue(viewModel.IsCurrentEncoding(Encoding.BigEndianUnicode));
    }

    [TestMethod]
    public void ChangeEncoding_FiresStatusChangedWithUpdatedEncoding()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"], new UTF8Encoding(false));
        viewModel.SetViewportHeight(5);
        viewModel.Refresh();

        StatusDetails? received = null;
        viewModel.StatusChanged += s => received = s;

        // act
        viewModel.ChangeEncoding(Encoding.Unicode);

        // assert
        Assert.IsNotNull(received);
        Assert.AreEqual("UTF-16", received.DocumentEncoding);
    }

    [TestMethod]
    public void ChangeEncoding_MarksDocumentDirty()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"], new UTF8Encoding(false));
        viewModel.SetViewportHeight(5);
        Assert.IsFalse(viewModel.Session.Document.IsModified);

        // act
        viewModel.ChangeEncoding(Encoding.Unicode);

        // assert
        Assert.IsTrue(viewModel.Session.Document.IsModified);
    }

    [TestMethod]
    public void IsCurrentEncoding_WhenEncodingMatches_ReturnsTrue()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"], new UTF8Encoding(false));

        // act & assert
        Assert.IsTrue(viewModel.IsCurrentEncoding(new UTF8Encoding(false)));
    }

    [TestMethod]
    public void IsCurrentEncoding_WhenEncodingDiffers_ReturnsFalse()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"], new UTF8Encoding(false));

        // act & assert
        Assert.IsFalse(viewModel.IsCurrentEncoding(Encoding.Unicode));
    }

    [TestMethod]
    public void IsCurrentEncoding_Utf8VsUtf8Bom_AreDistinct()
    {
        // arrange - start with BOM encoding
        var (viewModel, _) = CreateViewModel(["Hello"], new UTF8Encoding(true));

        // act & assert
        Assert.IsTrue(viewModel.IsCurrentEncoding(new UTF8Encoding(true)));
        Assert.IsFalse(viewModel.IsCurrentEncoding(new UTF8Encoding(false)));
    }

    [ExcludeFromCodeCoverage]
    private static (EditorViewModel viewModel, EditorSession session) CreateViewModel(
        string[] lines,
        Encoding? encoding = null)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(
            new DocumentData(textLines, encoding ?? new UTF8Encoding(false), LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var viewModel = new EditorViewModel(
            session,
            new EditorCommandService(session),
            EditorSettings.Default,
            new MockClipboardService());
        return (viewModel, session);
    }
}
