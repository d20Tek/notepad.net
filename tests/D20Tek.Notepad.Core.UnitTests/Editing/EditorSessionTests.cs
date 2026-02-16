using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.Editing;

[TestClass]
public partial class EditorSessionTests
{
    [TestMethod]
    public void Constructor_WithValidDocument_SetsDocumentProperty()
    {
        // arrange
        var doc = new Doc.Document([new("Hello")]);

        // act
        var session = new EditorSession(doc);

        // assert
        Assert.AreSame(doc, session.Document);
    }

    [TestMethod]
    public void Constructor_WithValidDocument_InitializesCaretAtOrigin()
    {
        // arrange
        var doc = new Doc.Document([new("Hello")]);

        // act
        var session = new EditorSession(doc);

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
        Assert.AreEqual(new TextPosition(0, 0), session.Anchor);
    }

    [TestMethod]
    public void Constructor_WithValidDocument_InitializesUndoStack()
    {
        // arrange
        var doc = new Doc.Document([new("Hello")]);

        // act
        var session = new EditorSession(doc);

        // assert
        Assert.IsNotNull(session.UndoStack);
    }

    [TestMethod]
    public void Execute_WithOperation_PushesToUndoStackAndExecutes()
    {
        // arrange
        var session = CreateSession("Hello");
        var op = new InsertTextOperation(new TextPosition(0, 5), " World", "", new TextPosition(0, 5));

        // act
        session.Execute(op);

        // assert
        Assert.AreEqual("Hello World", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Undo_AfterInsert_ReversesInsert()
    {
        // arrange
        var session = CreateSession("Hello");
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);
        session.InsertText(" World");

        // act
        session.Undo();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Redo_AfterUndo_ReappliesChange()
    {
        // arrange
        var session = CreateSession("Hello");
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);
        session.InsertText(" World");
        session.Undo();

        // act
        session.Redo();

        // assert
        Assert.AreEqual("Hello World", session.Document.Lines[0].Content);
    }

    private static EditorSession CreateSession(string content)
    {
        var doc = new Doc.Document([new(content)]);
        return new EditorSession(doc);
    }
}