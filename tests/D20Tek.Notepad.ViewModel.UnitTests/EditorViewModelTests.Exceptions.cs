using D20Tek.Notepad.Core.Document;
using D20Tek.Notepad.Core.Editing;
using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelExceptionsTests
{
    private static readonly DocumentFactory _docFactory = new();

    [TestMethod]
    public void Constructor_WithNullSession_ThrowsArgumentNullException()
    {
        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => new EditorViewModel(null!, null!));
    }

    [TestMethod]
    public void Constructor_WithNullCommandService_ThrowsArgumentNullException()
    {
        // arrange
        var session = CreateSession(["test"]);

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => new EditorViewModel(session, null!));
    }

    private static EditorSession CreateSession(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        return new EditorSession(doc);
    }
}
