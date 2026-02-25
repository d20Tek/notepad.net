using D20Tek.Notepad.Core;
using D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelFileStateTests
{
    private static readonly DocumentFactory _docFactory = new();

    // IsDirty Tests
    [TestMethod]
    public void IsDirty_InitialValue_IsFalse()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);

        // act & assert
        Assert.IsFalse(viewModel.IsDirty);
    }

    [TestMethod]
    public void IsDirty_AfterTyping_IsTrue()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.TypeCharacter('!');

        // assert
        Assert.IsTrue(viewModel.IsDirty);
    }

    [TestMethod]
    public void ClearDirtyFlag_ResetsIsDirty()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(5);
        viewModel.TypeCharacter('!');
        Assert.IsTrue(viewModel.IsDirty);

        // act
        viewModel.ClearDirtyFlag();

        // assert
        Assert.IsFalse(viewModel.IsDirty);
    }

    [TestMethod]
    public void MarkDirty_SetsIsDirtyToTrue()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);
        Assert.IsFalse(viewModel.IsDirty);

        // act
        viewModel.MarkDirty();

        // assert
        Assert.IsTrue(viewModel.IsDirty);
    }

    // CurrentFilePath Tests
    [TestMethod]
    public void CurrentFilePath_InitialValue_IsNull()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);

        // act & assert
        Assert.IsNull(viewModel.CurrentFilePath);
    }

    [TestMethod]
    public void SetFilePath_SetsCurrentFilePath()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);

        // act
        viewModel.SetFilePath(@"C:\test\file.txt");

        // assert
        Assert.AreEqual(@"C:\test\file.txt", viewModel.CurrentFilePath);
    }

    [TestMethod]
    public void SetFilePath_WithNullPath_ThrowsArgumentException()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => viewModel.SetFilePath(null!));
    }

    [TestMethod]
    public void SetFilePath_WithEmptyPath_ThrowsArgumentException()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);

        // act & assert
        Assert.ThrowsExactly<ArgumentException>(
            [ExcludeFromCodeCoverage] () => viewModel.SetFilePath(string.Empty));
    }

    [TestMethod]
    public void SetFilePath_WithWhitespacePath_ThrowsArgumentException()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);

        // act & assert
        Assert.ThrowsExactly<ArgumentException>(
            [ExcludeFromCodeCoverage] () => viewModel.SetFilePath("   "));
    }

    [TestMethod]
    public void ClearFilePath_SetsCurrentFilePathToNull()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);
        viewModel.SetFilePath(@"C:\test\file.txt");

        // act
        viewModel.ClearFilePath();

        // assert
        Assert.IsNull(viewModel.CurrentFilePath);
    }

    // DocumentTitle Tests
    [TestMethod]
    public void DocumentTitle_WithNoFilePath_ReturnsUntitled()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);

        // act & assert
        Assert.AreEqual("Untitled", viewModel.DocumentTitle);
    }

    [TestMethod]
    public void DocumentTitle_WithFilePath_ReturnsFileName()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);
        viewModel.SetFilePath(@"C:\test\myfile.txt");

        // act & assert
        Assert.AreEqual("myfile.txt", viewModel.DocumentTitle);
    }

    // Events Tests
    [TestMethod]
    public void DirtyStateChanged_FiredWhenDirtyChanges()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(5);
        bool? eventValue = null;
        viewModel.DirtyStateChanged += (dirty) => eventValue = dirty;

        // act
        viewModel.TypeCharacter('!');

        // assert
        Assert.IsTrue(eventValue);
    }

    [TestMethod]
    public void DirtyStateChanged_NotFiredWhenValueSame()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(5);
        viewModel.TypeCharacter('!');
        int fireCount = 0;
        viewModel.DirtyStateChanged += [ExcludeFromCodeCoverage](_) => fireCount++;

        // act - mark dirty again (already dirty)
        viewModel.MarkDirty();

        // assert
        Assert.AreEqual(0, fireCount);
    }

    [TestMethod]
    public void FilePathChanged_FiredWhenPathChanges()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);
        string? eventValue = null;
        viewModel.FilePathChanged += (path) => eventValue = path;

        // act
        viewModel.SetFilePath(@"C:\test\file.txt");

        // assert
        Assert.AreEqual(@"C:\test\file.txt", eventValue);
    }

    [TestMethod]
    public void FilePathChanged_NotFiredWhenValueSame()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);
        viewModel.SetFilePath(@"C:\test\file.txt");
        int fireCount = 0;
        viewModel.FilePathChanged += [ExcludeFromCodeCoverage](_) => fireCount++;

        // act - set same path
        viewModel.SetFilePath(@"C:\test\file.txt");

        // assert
        Assert.AreEqual(0, fireCount);
    }

    private static (EditorViewModel viewModel, EditorSession session) CreateViewModel(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new DocumentData(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var viewModel = new EditorViewModel(session, new EditorCommandService(session));
        return (viewModel, session);
    }
}
