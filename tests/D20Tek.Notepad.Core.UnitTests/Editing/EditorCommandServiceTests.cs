using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.Editing;

[TestClass]
public partial class EditorCommandServiceTests
{
    [TestMethod]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // arrange
        var session = CreateSession(["Hello"]);

        // act
        var service = new EditorCommandService(session);

        // assert
        Assert.IsNotNull(service);
    }

    [TestMethod]
    public void Undo_ReversesLastOperation()
    {
        // arrange
        var (service, session) = CreateService(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);
        service.InsertText(" World");

        // act
        service.Undo();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Redo_ReappliesUndoneOperation()
    {
        // arrange
        var (service, session) = CreateService(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);
        service.InsertText(" World");
        service.Undo();

        // act
        service.Redo();

        // assert
        Assert.AreEqual("Hello World", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void BeginTypingGroup_AndEndTypingGroup_GroupsOperations()
    {
        // arrange
        var (service, session) = CreateService(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        service.BeginTypingGroup();
        service.TypeCharacter('1');
        service.TypeCharacter('2');
        service.TypeCharacter('3');
        service.EndTypingGroup();
        service.Undo();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void SelectAll_SelectsEntireDocument()
    {
        // arrange
        var (service, session) = CreateService(["Hello", "World", "Test"]);
        session.Caret = new TextPosition(1, 2);
        session.Anchor = new TextPosition(1, 2);

        // act
        service.SelectAll();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Anchor);
        Assert.AreEqual(new TextPosition(2, 4), session.Caret);
        Assert.IsTrue(session.HasSelection);
    }

    private static EditorSession CreateSession(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToArray();
        var doc = new Doc.Document(textLines);
        return new EditorSession(doc);
    }

    private static (EditorCommandService service, EditorSession session) CreateService(string[] lines)
    {
        var session = CreateSession(lines);
        var service = new EditorCommandService(session);
        return (service, session);
    }
}