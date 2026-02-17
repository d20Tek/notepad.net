using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.Editing;

[TestClass]
[ExcludeFromCodeCoverage]
public class EditorCommandServiceExceptionsTests
{
    [TestMethod]
    public void Constructor_WithNullSession_ThrowsArgumentNullException()
    {
        // arrange
        var navigation = new EditorNavigationService();

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(() => new EditorCommandService(null!, navigation));
    }

    [TestMethod]
    public void Constructor_WithNullNavigation_ThrowsArgumentNullException()
    {
        // arrange
        var session = CreateSession(["Hello"]);

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(() => new EditorCommandService(session, null!));
    }

    private static EditorSession CreateSession(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToArray();
        var doc = new Doc.Document(textLines);
        return new EditorSession(doc);
    }
}