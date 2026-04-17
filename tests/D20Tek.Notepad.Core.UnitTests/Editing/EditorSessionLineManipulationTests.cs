using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.Editing;

[TestClass]
public class EditorSessionLineManipulationTests
{
    // DuplicateLine Tests
    [TestMethod]
    public void DuplicateLine_WithNoSelection_DuplicatesCurrentLine()
    {
        // arrange
        var session = CreateMultiLineSession(["Hello", "World"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        session.DuplicateLine();

        // assert
        Assert.AreEqual(3, session.Document.Lines.Count);
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
        Assert.AreEqual("Hello", session.Document.Lines[1].Content);
        Assert.AreEqual("World", session.Document.Lines[2].Content);
    }

    [TestMethod]
    public void DuplicateLine_WithNoSelection_MovesCaretToNewLine()
    {
        // arrange
        var session = CreateMultiLineSession(["Hello", "World"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        session.DuplicateLine();

        // assert
        Assert.AreEqual(new TextPosition(1, 3), session.Caret);
    }

    [TestMethod]
    public void DuplicateLine_WithSelection_DuplicatesSelectedText()
    {
        // arrange
        var session = CreateMultiLineSession(["Hello", "World"]);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 5);

        // act
        session.DuplicateLine();

        // assert
        Assert.AreEqual(3, session.Document.Lines.Count);
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
        Assert.AreEqual("Hello", session.Document.Lines[1].Content);
    }

    [TestMethod]
    public void DuplicateLine_IsUndoable()
    {
        // arrange
        var session = CreateMultiLineSession(["Hello", "World"]);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);
        session.DuplicateLine();

        // act
        session.Undo();

        // assert
        Assert.AreEqual(2, session.Document.Lines.Count);
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
        Assert.AreEqual("World", session.Document.Lines[1].Content);
    }

    [TestMethod]
    public void DuplicateLine_OnLastLine_DuplicatesCorrectly()
    {
        // arrange
        var session = CreateMultiLineSession(["Hello", "World"]);
        session.Caret = new TextPosition(1, 2);
        session.Anchor = new TextPosition(1, 2);

        // act
        session.DuplicateLine();

        // assert
        Assert.AreEqual(3, session.Document.Lines.Count);
        Assert.AreEqual("World", session.Document.Lines[1].Content);
        Assert.AreEqual("World", session.Document.Lines[2].Content);
    }

    // MoveLineUp Tests
    [TestMethod]
    public void MoveLineUp_SwapsWithPreviousLine()
    {
        // arrange
        var session = CreateMultiLineSession(["First", "Second", "Third"]);
        session.Caret = new TextPosition(1, 2);
        session.Anchor = new TextPosition(1, 2);

        // act
        session.MoveLineUp();

        // assert
        Assert.AreEqual("Second", session.Document.Lines[0].Content);
        Assert.AreEqual("First", session.Document.Lines[1].Content);
        Assert.AreEqual("Third", session.Document.Lines[2].Content);
    }

    [TestMethod]
    public void MoveLineUp_MovesCaretUp()
    {
        // arrange
        var session = CreateMultiLineSession(["First", "Second"]);
        session.Caret = new TextPosition(1, 3);
        session.Anchor = new TextPosition(1, 3);

        // act
        session.MoveLineUp();

        // assert
        Assert.AreEqual(0, session.Caret.Line);
    }

    [TestMethod]
    public void MoveLineUp_AtFirstLine_DoesNothing()
    {
        // arrange
        var session = CreateMultiLineSession(["First", "Second"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);

        // act
        session.MoveLineUp();

        // assert
        Assert.AreEqual("First", session.Document.Lines[0].Content);
        Assert.AreEqual("Second", session.Document.Lines[1].Content);
    }

    [TestMethod]
    public void MoveLineUp_IsUndoable()
    {
        // arrange
        var session = CreateMultiLineSession(["First", "Second"]);
        session.Caret = new TextPosition(1, 0);
        session.Anchor = new TextPosition(1, 0);
        session.MoveLineUp();

        // act
        session.Undo();

        // assert
        Assert.AreEqual("First", session.Document.Lines[0].Content);
        Assert.AreEqual("Second", session.Document.Lines[1].Content);
    }

    [TestMethod]
    public void MoveLineUp_ClampsCaretColumn()
    {
        // arrange
        var session = CreateMultiLineSession(["Hi", "LongLine"]);
        session.Caret = new TextPosition(1, 7);
        session.Anchor = new TextPosition(1, 7);

        // act
        session.MoveLineUp();

        // assert
        Assert.AreEqual(0, session.Caret.Line);
        Assert.AreEqual(7, session.Caret.Column);
    }

    // MoveLineDown Tests
    [TestMethod]
    public void MoveLineDown_SwapsWithNextLine()
    {
        // arrange
        var session = CreateMultiLineSession(["First", "Second", "Third"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);

        // act
        session.MoveLineDown();

        // assert
        Assert.AreEqual("Second", session.Document.Lines[0].Content);
        Assert.AreEqual("First", session.Document.Lines[1].Content);
        Assert.AreEqual("Third", session.Document.Lines[2].Content);
    }

    [TestMethod]
    public void MoveLineDown_MovesCaretDown()
    {
        // arrange
        var session = CreateMultiLineSession(["First", "Second"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        session.MoveLineDown();

        // assert
        Assert.AreEqual(1, session.Caret.Line);
    }

    [TestMethod]
    public void MoveLineDown_AtLastLine_DoesNothing()
    {
        // arrange
        var session = CreateMultiLineSession(["First", "Second"]);
        session.Caret = new TextPosition(1, 2);
        session.Anchor = new TextPosition(1, 2);

        // act
        session.MoveLineDown();

        // assert
        Assert.AreEqual("First", session.Document.Lines[0].Content);
        Assert.AreEqual("Second", session.Document.Lines[1].Content);
    }

    [TestMethod]
    public void MoveLineDown_IsUndoable()
    {
        // arrange
        var session = CreateMultiLineSession(["First", "Second"]);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);
        session.MoveLineDown();

        // act
        session.Undo();

        // assert
        Assert.AreEqual("First", session.Document.Lines[0].Content);
        Assert.AreEqual("Second", session.Document.Lines[1].Content);
    }

    // IndentLines Tests
    [TestMethod]
    public void IndentLines_WithNoSelection_IndentsCurrentLine()
    {
        // arrange
        var session = CreateMultiLineSession(["Hello", "World"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);

        // act
        session.IndentLines("    ");

        // assert
        Assert.AreEqual("    Hello", session.Document.Lines[0].Content);
        Assert.AreEqual("World", session.Document.Lines[1].Content);
    }

    [TestMethod]
    public void IndentLines_WithMultiLineSelection_IndentsAllLines()
    {
        // arrange
        var session = CreateMultiLineSession(["Line1", "Line2", "Line3"]);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(2, 3);

        // act
        session.IndentLines("    ");

        // assert
        Assert.AreEqual("    Line1", session.Document.Lines[0].Content);
        Assert.AreEqual("    Line2", session.Document.Lines[1].Content);
        Assert.AreEqual("    Line3", session.Document.Lines[2].Content);
    }

    [TestMethod]
    public void IndentLines_WithTabCharacter_IndentsWithTab()
    {
        // arrange
        var session = CreateMultiLineSession(["Hello"]);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);

        // act
        session.IndentLines("\t");

        // assert
        Assert.AreEqual("\tHello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void IndentLines_IsUndoable()
    {
        // arrange
        var session = CreateMultiLineSession(["Hello", "World"]);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(1, 5);
        session.IndentLines("    ");

        // act
        session.Undo();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
        Assert.AreEqual("World", session.Document.Lines[1].Content);
    }

    // OutdentLines Tests
    [TestMethod]
    public void OutdentLines_WithFullIndent_RemovesIndent()
    {
        // arrange
        var session = CreateMultiLineSession(["    Hello", "    World"]);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(1, 9);

        // act
        session.OutdentLines("    ");

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
        Assert.AreEqual("World", session.Document.Lines[1].Content);
    }

    [TestMethod]
    public void OutdentLines_WithPartialIndent_RemovesAvailableWhitespace()
    {
        // arrange
        var session = CreateMultiLineSession(["  Hello"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);

        // act
        session.OutdentLines("    ");

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void OutdentLines_WithNoIndent_DoesNothing()
    {
        // arrange
        var session = CreateMultiLineSession(["Hello"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);

        // act
        session.OutdentLines("    ");

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void OutdentLines_IsUndoable()
    {
        // arrange
        var session = CreateMultiLineSession(["    Hello"]);
        session.Caret = new TextPosition(0, 4);
        session.Anchor = new TextPosition(0, 4);
        session.OutdentLines("    ");

        // act
        session.Undo();

        // assert
        Assert.AreEqual("    Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void OutdentLines_WithTabIndent_RemovesTab()
    {
        // arrange
        var session = CreateMultiLineSession(["\tHello"]);
        session.Caret = new TextPosition(0, 1);
        session.Anchor = new TextPosition(0, 1);

        // act
        session.OutdentLines("\t");

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    // TrimTrailingWhitespace Tests
    [TestMethod]
    public void TrimTrailingWhitespace_RemovesTrailingSpaces()
    {
        // arrange
        var session = CreateMultiLineSession(["Hello   ", "World  ", "Test"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        session.TrimTrailingWhitespace();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
        Assert.AreEqual("World", session.Document.Lines[1].Content);
        Assert.AreEqual("Test", session.Document.Lines[2].Content);
    }

    [TestMethod]
    public void TrimTrailingWhitespace_RemovesTrailingTabs()
    {
        // arrange
        var session = CreateMultiLineSession(["Hello\t\t"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        session.TrimTrailingWhitespace();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void TrimTrailingWhitespace_WithNoTrailing_DoesNothing()
    {
        // arrange
        var session = CreateMultiLineSession(["Hello", "World"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        session.TrimTrailingWhitespace();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
        Assert.AreEqual("World", session.Document.Lines[1].Content);
        Assert.IsFalse(session.UndoStack.CanUndo);
    }

    [TestMethod]
    public void TrimTrailingWhitespace_IsUndoable()
    {
        // arrange
        var session = CreateMultiLineSession(["Hello   ", "World  "]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);
        session.TrimTrailingWhitespace();

        // act
        session.Undo();

        // assert
        Assert.AreEqual("Hello   ", session.Document.Lines[0].Content);
        Assert.AreEqual("World  ", session.Document.Lines[1].Content);
    }

    [TestMethod]
    public void TrimTrailingWhitespace_ClampsCaretIfBeyondTrimmedLine()
    {
        // arrange
        var session = CreateMultiLineSession(["Hello   "]);
        session.Caret = new TextPosition(0, 8);
        session.Anchor = new TextPosition(0, 8);

        // act
        session.TrimTrailingWhitespace();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
        Assert.AreEqual(5, session.Caret.Column);
    }

    [TestMethod]
    public void TrimTrailingWhitespace_RemovesMixedWhitespace()
    {
        // arrange
        var session = CreateMultiLineSession(["Hello \t  "]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        session.TrimTrailingWhitespace();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    private static EditorSession CreateMultiLineSession(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToArray();
        var doc = new Doc.Document(textLines);
        return new EditorSession(doc);
    }
}
