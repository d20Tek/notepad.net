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

    // ReplaceDocument tests
    [TestMethod]
    public void ReplaceDocument_WithNewDocument_UpdatesDocumentProperty()
    {
        // arrange
        var session = CreateSession("Original");
        var newDoc = new Doc.Document([new("New Document")]);

        // act
        session.ReplaceDocument(newDoc);

        // assert
        Assert.AreSame(newDoc, session.Document);
        Assert.AreEqual("New Document", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void ReplaceDocument_ResetsCaretToOrigin()
    {
        // arrange
        var session = CreateSession("Hello World");
        session.Caret = new TextPosition(0, 8);
        session.Anchor = new TextPosition(0, 8);
        var newDoc = new Doc.Document([new("New")]);

        // act
        session.ReplaceDocument(newDoc);

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
    }

    [TestMethod]
    public void ReplaceDocument_ClearsSelection()
    {
        // arrange
        var session = CreateSession("Hello World");
        session.Anchor = new TextPosition(0, 2);
        session.Caret = new TextPosition(0, 8);
        var newDoc = new Doc.Document([new("New")]);

        // act
        session.ReplaceDocument(newDoc);

        // assert
        Assert.IsFalse(session.HasSelection);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void ReplaceDocument_WithMultiLineDocument_UpdatesCorrectly()
    {
        // arrange
        var session = CreateSession("Original");
        var newDoc = new Doc.Document([new("Line 1"), new("Line 2"), new("Line 3")]);

        // act
        session.ReplaceDocument(newDoc);

        // assert
        Assert.AreEqual(3, session.Document.Lines.Count);
        Assert.AreEqual("Line 1", session.Document.Lines[0].Content);
        Assert.AreEqual("Line 2", session.Document.Lines[1].Content);
        Assert.AreEqual("Line 3", session.Document.Lines[2].Content);
    }

    [TestMethod]
    public void ReplaceDocument_AfterEdits_ReplacesModifiedDocument()
    {
        // arrange
        var session = CreateSession("Hello");
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);
        session.InsertText(" World");
        Assert.AreEqual("Hello World", session.Document.Lines[0].Content);

        var newDoc = new Doc.Document([new("Fresh Start")]);

        // act
        session.ReplaceDocument(newDoc);

        // assert
        Assert.AreEqual("Fresh Start", session.Document.Lines[0].Content);
    }

    private static EditorSession CreateSession(string content)
    {
        var doc = new Doc.Document([new(content)]);
        return new EditorSession(doc);
    }
}