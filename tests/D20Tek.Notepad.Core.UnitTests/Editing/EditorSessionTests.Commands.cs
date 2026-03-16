using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.Editing;

public partial class EditorSessionTests
{
    [TestMethod]
    public void InsertText_WithNoSelection_InsertsTextAtCaret()
    {
        // arrange
        var session = CreateSession("Hello");
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        session.InsertText(" World");

        // assert
        Assert.AreEqual("Hello World", session.Document.Lines[0].Content);
        Assert.AreEqual(new TextPosition(0, 11), session.Caret);
    }

    [TestMethod]
    public void InsertText_WithSelection_ReplacesSelection()
    {
        // arrange
        var session = CreateSession("Hello World");
        session.Anchor = new TextPosition(0, 6);
        session.Caret = new TextPosition(0, 11);

        // act
        session.InsertText("Universe");

        // assert
        Assert.AreEqual("Hello Universe", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void InsertText_AddsOperationToUndoStack()
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
    public void ReplaceSelection_WithSelection_ReplacesSelectedText()
    {
        // arrange
        var session = CreateSession("Hello World");
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 5);

        // act
        session.ReplaceSelection("Hi");

        // assert
        Assert.AreEqual("Hi World", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void DeleteSelection_WithSelection_DeletesSelectedText()
    {
        // arrange
        var session = CreateSession("Hello World");
        session.Anchor = new TextPosition(0, 5);
        session.Caret = new TextPosition(0, 11);

        // act
        session.DeleteSelection();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void DeleteSelection_WithNoSelection_DoesNothing()
    {
        // arrange
        var session = CreateSession("Hello");

        // act
        session.DeleteSelection();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Backspace_WithSelection_DeletesSelection()
    {
        // arrange
        var session = CreateSession("Hello World");
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 6);

        // act
        session.Backspace();

        // assert
        Assert.AreEqual("World", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Backspace_AtMiddleOfLine_DeletesPreviousCharacter()
    {
        // arrange
        var session = CreateSession("Hello");
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        session.Backspace();

        // assert
        Assert.AreEqual("Hell", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Backspace_AtStartOfLine_JoinsWithPreviousLine()
    {
        // arrange
        var doc = new Doc.Document([new("Hello"), new("World")]);
        var session = new EditorSession(doc);
        session.Caret = new TextPosition(1, 0);
        session.Anchor = new TextPosition(1, 0);

        // act
        session.Backspace();

        // assert
        Assert.AreEqual(1, doc.Lines.Count);
        Assert.AreEqual("HelloWorld", doc.Lines[0].Content);
    }

    [TestMethod]
    public void Backspace_AtDocumentStart_DoesNothing()
    {
        // arrange
        var session = CreateSession("Hello");
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);

        // act
        session.Backspace();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Delete_WithSelection_DeletesSelection()
    {
        // arrange
        var session = CreateSession("Hello World");
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 6);

        // act
        session.Delete();

        // assert
        Assert.AreEqual("World", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Delete_AtMiddleOfLine_DeletesNextCharacter()
    {
        // arrange
        var session = CreateSession("Hello");
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);

        // act
        session.Delete();

        // assert
        Assert.AreEqual("ello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Delete_AtEndOfLine_JoinsWithNextLine()
    {
        // arrange
        var doc = new Doc.Document([new("Hello"), new("World")]);
        var session = new EditorSession(doc);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        session.Delete();

        // assert
        Assert.AreEqual(1, doc.Lines.Count);
        Assert.AreEqual("HelloWorld", doc.Lines[0].Content);
    }

    [TestMethod]
    public void Delete_AtDocumentEnd_DoesNothing()
    {
        // arrange
        var session = CreateSession("Hello");
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        session.Delete();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void InsertNewLine_AtMiddleOfLine_SplitsLine()
    {
        // arrange
        var session = CreateSession("HelloWorld");
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        session.InsertNewLine();

        // assert
        Assert.AreEqual(2, session.Document.Lines.Count);
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
        Assert.AreEqual("World", session.Document.Lines[1].Content);
    }

    [TestMethod]
    public void OverwriteCharacter_MidLine_ReplacesCharAtCaret()
    {
        // arrange
        var session = CreateSession("Hello");
        session.Caret = new TextPosition(0, 1);
        session.Anchor = new TextPosition(0, 1);

        // act
        session.OverwriteCharacter('X');

        // assert
        Assert.AreEqual("HXllo", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void OverwriteCharacter_MidLine_AdvancesCaretByOne()
    {
        // arrange
        var session = CreateSession("Hello");
        session.Caret = new TextPosition(0, 1);
        session.Anchor = new TextPosition(0, 1);

        // act
        session.OverwriteCharacter('X');

        // assert
        Assert.AreEqual(new TextPosition(0, 2), session.Caret);
    }

    [TestMethod]
    public void OverwriteCharacter_AtLineEnd_InsertsCharacter()
    {
        // arrange
        var session = CreateSession("Hello");
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        session.OverwriteCharacter('!');

        // assert
        Assert.AreEqual("Hello!", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void OverwriteCharacter_WithSelection_ReplacesSelection()
    {
        // arrange
        var session = CreateSession("Hello World");
        session.Anchor = new TextPosition(0, 6);
        session.Caret = new TextPosition(0, 11);

        // act
        session.OverwriteCharacter('X');

        // assert
        Assert.AreEqual("Hello X", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void OverwriteCharacter_IsUndoable()
    {
        // arrange
        var session = CreateSession("Hello");
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);
        session.OverwriteCharacter('X');

        // act
        session.Undo();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }
}

