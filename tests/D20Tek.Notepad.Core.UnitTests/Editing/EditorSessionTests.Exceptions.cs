using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.Editing;

[TestClass]
[ExcludeFromCodeCoverage]
public class EditorSessionExceptionTests
{
    [TestMethod]
    public void Constructor_WithNullDocument_ThrowsArgumentNullException()
    {
        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(() => new EditorSession(null!));
    }

    [TestMethod]
    public void Execute_WithNullOperation_ThrowsArgumentNullException()
    {
        // arrange
        var session = CreateSession("Hello");

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(() => session.Execute(null!));
    }

    [TestMethod]
    public void InsertText_WithNullText_ThrowsArgumentNullException()
    {
        // arrange
        var session = CreateSession("Hello");

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(() => session.InsertText(null!));
    }

    [TestMethod]
    public void ReplaceSelection_WithNullText_ThrowsArgumentNullException()
    {
        // arrange
        var session = CreateSession("Hello");

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(() => session.ReplaceSelection(null!));
    }

    private static EditorSession CreateSession(string content)
    {
        var doc = new Doc.Document([new(content)]);
        return new EditorSession(doc);
    }
}