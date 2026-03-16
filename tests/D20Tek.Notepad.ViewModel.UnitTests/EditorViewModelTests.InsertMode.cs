namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelInsertModeTests
{
    private static readonly DocumentFactory _docFactory = new();

    [TestMethod]
    public void IsOverwriteMode_Default_IsFalse()
    {
        // arrange
        var viewModel = CreateViewModel(["Hello"]);

        // act & assert
        Assert.IsFalse(viewModel.IsOverwriteMode);
    }

    [TestMethod]
    public void ToggleInsertMode_WhenInsertMode_SetsOverwriteMode()
    {
        // arrange
        var viewModel = CreateViewModel(["Hello"]);

        // act
        viewModel.ToggleInsertMode();

        // assert
        Assert.IsTrue(viewModel.IsOverwriteMode);
    }

    [TestMethod]
    public void ToggleInsertMode_WhenOverwriteMode_SetsInsertMode()
    {
        // arrange
        var viewModel = CreateViewModel(["Hello"]);
        viewModel.ToggleInsertMode();

        // act
        viewModel.ToggleInsertMode();

        // assert
        Assert.IsFalse(viewModel.IsOverwriteMode);
    }

    [TestMethod]
    public void ToggleInsertMode_FiresInsertModeChangedEvent()
    {
        // arrange
        var viewModel = CreateViewModel(["Hello"]);
        bool? eventValue = null;
        viewModel.InsertModeChanged += isOverwrite => eventValue = isOverwrite;

        // act
        viewModel.ToggleInsertMode();

        // assert
        Assert.IsNotNull(eventValue);
        Assert.IsTrue(eventValue);
    }

    [TestMethod]
    public void ToggleInsertMode_FiresStatusChangedEvent()
    {
        // arrange
        var viewModel = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(5);
        viewModel.Refresh();
        StatusDetails? eventValue = null;
        viewModel.StatusChanged += s => eventValue = s;

        // act
        viewModel.ToggleInsertMode();

        // assert
        Assert.IsNotNull(eventValue);
        Assert.IsTrue(eventValue.IsOverwriteMode);
    }

    [TestMethod]
    public void TypeCharacter_WhenInsertMode_InsertsChar()
    {
        // arrange
        var viewModel = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(5);
        viewModel.Session.Caret = new TextPosition(0, 2);
        viewModel.Session.Anchor = new TextPosition(0, 2);

        // act
        viewModel.TypeCharacter('X');

        // assert
        Assert.AreEqual("HeXllo", viewModel.Session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void TypeCharacter_WhenOverwriteMode_MidLine_ReplacesChar()
    {
        // arrange
        var viewModel = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(5);
        viewModel.ToggleInsertMode();
        viewModel.Session.Caret = new TextPosition(0, 2);
        viewModel.Session.Anchor = new TextPosition(0, 2);

        // act
        viewModel.TypeCharacter('X');

        // assert
        Assert.AreEqual("HeXlo", viewModel.Session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void TypeCharacter_WhenOverwriteMode_AtLineEnd_AppendsChar()
    {
        // arrange
        var viewModel = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(5);
        viewModel.ToggleInsertMode();
        viewModel.Session.Caret = new TextPosition(0, 5);
        viewModel.Session.Anchor = new TextPosition(0, 5);

        // act
        viewModel.TypeCharacter('!');

        // assert
        Assert.AreEqual("Hello!", viewModel.Session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void TypeCharacter_WhenOverwriteMode_WithSelection_ReplacesSelection()
    {
        // arrange
        var viewModel = CreateViewModel(["Hello World"]);
        viewModel.SetViewportHeight(5);
        viewModel.ToggleInsertMode();
        viewModel.Session.Anchor = new TextPosition(0, 6);
        viewModel.Session.Caret = new TextPosition(0, 11);

        // act
        viewModel.TypeCharacter('X');

        // assert
        Assert.AreEqual("Hello X", viewModel.Session.Document.Lines[0].Content);
    }

    private static EditorViewModel CreateViewModel(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        return new EditorViewModel(session, new EditorCommandService(session),
            EditorSettings.Default, new MockClipboardService());
    }
}
