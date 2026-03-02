using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.Editing;

[TestClass]
public class CaretNavigatorWordTests
{
    // MoveWordLeft Tests
    [TestMethod]
    public void MoveWordLeft_FromMiddleOfWord_MovesToWordStart()
    {
        // arrange
        var session = CreateSession(["Hello World"]);
        session.Caret = new TextPosition(0, 8);
        session.Anchor = new TextPosition(0, 8);

        // act
        session.Navigator.MoveWordLeft();

        // assert
        Assert.AreEqual(new TextPosition(0, 6), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MoveWordLeft_FromWordStart_MovesToPreviousWordStart()
    {
        // arrange
        var session = CreateSession(["Hello World"]);
        session.Caret = new TextPosition(0, 6);
        session.Anchor = new TextPosition(0, 6);

        // act
        session.Navigator.MoveWordLeft();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MoveWordLeft_SkipsWhitespace()
    {
        // arrange
        var session = CreateSession(["Hello   World"]);
        session.Caret = new TextPosition(0, 8);
        session.Anchor = new TextPosition(0, 8);

        // act
        session.Navigator.MoveWordLeft();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
    }

    [TestMethod]
    public void MoveWordLeft_SkipsPunctuation()
    {
        // arrange
        var session = CreateSession(["foo.bar"]);
        session.Caret = new TextPosition(0, 4);
        session.Anchor = new TextPosition(0, 4);

        // act
        session.Navigator.MoveWordLeft();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
    }

    [TestMethod]
    public void MoveWordLeft_AtLineStart_MovesToEndOfPreviousLine()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(1, 0);
        session.Anchor = new TextPosition(1, 0);

        // act
        session.Navigator.MoveWordLeft();

        // assert
        Assert.AreEqual(new TextPosition(0, 5), session.Caret);
    }

    [TestMethod]
    public void MoveWordLeft_AtDocumentStart_StaysAtStart()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);

        // act
        session.Navigator.MoveWordLeft();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
    }

    [TestMethod]
    public void MoveWordLeft_MultiplePunctuation_SkipsAll()
    {
        // arrange
        var session = CreateSession(["foo===bar"]);
        session.Caret = new TextPosition(0, 6);
        session.Anchor = new TextPosition(0, 6);

        // act
        session.Navigator.MoveWordLeft();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
    }

    [TestMethod]
    public void MoveWordLeft_OnlyNonWordCharsBeforeCaret_MovesToLineStart()
    {
        // arrange - only non-word chars before caret, no word to stop at
        // covers: while (col > 0 && !IsWordChar(content[col - 1])) exiting via col == 0
        var session = CreateSession(["===foo"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        session.Navigator.MoveWordLeft();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
    }

    // MoveWordRight Tests
    [TestMethod]
    public void MoveWordRight_FromMiddleOfWord_MovesToNextWordStart()
    {
        // arrange
        var session = CreateSession(["Hello World"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);

        // act
        session.Navigator.MoveWordRight();

        // assert
        Assert.AreEqual(new TextPosition(0, 6), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MoveWordRight_SkipsWhitespace()
    {
        // arrange
        var session = CreateSession(["Hello   World"]);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);

        // act
        session.Navigator.MoveWordRight();

        // assert
        Assert.AreEqual(new TextPosition(0, 8), session.Caret);
    }

    [TestMethod]
    public void MoveWordRight_SkipsPunctuation()
    {
        // arrange
        var session = CreateSession(["foo.bar"]);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);

        // act
        session.Navigator.MoveWordRight();

        // assert
        Assert.AreEqual(new TextPosition(0, 4), session.Caret);
    }

    [TestMethod]
    public void MoveWordRight_AtLineEnd_MovesToStartOfNextLine()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        session.Navigator.MoveWordRight();

        // assert
        Assert.AreEqual(new TextPosition(1, 0), session.Caret);
    }

    [TestMethod]
    public void MoveWordRight_AtDocumentEnd_StaysAtEnd()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        session.Navigator.MoveWordRight();

        // assert
        Assert.AreEqual(new TextPosition(0, 5), session.Caret);
    }

    [TestMethod]
    public void MoveWordRight_MultiplePunctuation_SkipsAll()
    {
        // arrange
        var session = CreateSession(["foo===bar"]);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);

        // act
        session.Navigator.MoveWordRight();

        // assert
        Assert.AreEqual(new TextPosition(0, 6), session.Caret);
    }

    [TestMethod]
    public void MoveWordRight_WordReachesEndOfLine_StopsAtLineEnd()
    {
        // arrange - word fills to end of line with no trailing non-word chars
        // covers: while (col < content.Length && IsWordChar(content[col])) exiting via col >= content.Length
        var session = CreateSession(["Hello World"]);
        session.Caret = new TextPosition(0, 6);
        session.Anchor = new TextPosition(0, 6);

        // act
        session.Navigator.MoveWordRight();

        // assert
        Assert.AreEqual(new TextPosition(0, 11), session.Caret);
    }

    [TestMethod]
    public void MoveWordRight_NonWordCharsAtLineEnd_StopsAtLineEnd()
    {
        // arrange - non-word chars fill to end of line
        // covers: while (col < content.Length && !IsWordChar(content[col])) exiting via col >= content.Length
        var session = CreateSession(["foo..."]);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);

        // act
        session.Navigator.MoveWordRight();

        // assert
        Assert.AreEqual(new TextPosition(0, 6), session.Caret);
    }

    // ExtendWordLeft Tests
    [TestMethod]
    public void ExtendWordLeft_ExtendsSelectionToWordStart()
    {
        // arrange
        var session = CreateSession(["Hello World"]);
        session.Caret = new TextPosition(0, 8);
        session.Anchor = new TextPosition(0, 8);

        // act
        session.Navigator.ExtendWordLeft();

        // assert
        Assert.AreEqual(new TextPosition(0, 6), session.Caret);
        Assert.AreEqual(new TextPosition(0, 8), session.Anchor);
    }

    // ExtendWordRight Tests
    [TestMethod]
    public void ExtendWordRight_ExtendsSelectionToNextWordStart()
    {
        // arrange
        var session = CreateSession(["Hello World"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);

        // act
        session.Navigator.ExtendWordRight();

        // assert
        Assert.AreEqual(new TextPosition(0, 6), session.Caret);
        Assert.AreEqual(new TextPosition(0, 2), session.Anchor);
    }

    private static EditorSession CreateSession(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToArray();
        var doc = new Doc.Document(textLines);
        return new EditorSession(doc);
    }
}
