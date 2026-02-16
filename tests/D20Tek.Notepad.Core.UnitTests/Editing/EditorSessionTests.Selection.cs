using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.Editing;

public partial class EditorSessionTests
{
    [TestMethod]
    public void HasSelection_WhenCaretEqualsAnchor_ReturnsFalse()
    {
        // arrange
        var session = CreateSession("Hello");

        // act
        var result = session.HasSelection;

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void HasSelection_WhenCaretDiffersFromAnchor_ReturnsTrue()
    {
        // arrange
        var session = CreateSession("Hello");
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 5);

        // act
        var result = session.HasSelection;

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void GetSelectionRange_WithForwardSelection_ReturnsNormalizedRange()
    {
        // arrange
        var session = CreateSession("Hello World");
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 5);

        // act
        var range = session.GetSelectionRange();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), range.Start);
        Assert.AreEqual(new TextPosition(0, 5), range.End);
    }

    [TestMethod]
    public void GetSelectionRange_WithBackwardSelection_ReturnsNormalizedRange()
    {
        // arrange
        var session = CreateSession("Hello World");
        session.Anchor = new TextPosition(0, 5);
        session.Caret = new TextPosition(0, 0);

        // act
        var range = session.GetSelectionRange();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), range.Start);
        Assert.AreEqual(new TextPosition(0, 5), range.End);
    }

    [TestMethod]
    public void GetSelectedText_WithNoSelection_ReturnsEmptyString()
    {
        // arrange
        var session = CreateSession("Hello");

        // act
        var result = session.GetSelectedText();

        // assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void GetSelectedText_WithSingleLineSelection_ReturnsSelectedText()
    {
        // arrange
        var session = CreateSession("Hello World");
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 5);

        // act
        var result = session.GetSelectedText();

        // assert
        Assert.AreEqual("Hello", result);
    }

    [TestMethod]
    public void GetSelectedText_WithMultiLineSelection_ReturnsSelectedText()
    {
        // arrange
        var doc = new Doc.Document([new("Hello"), new("World"), new("Test")]);
        var session = new EditorSession(doc);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(2, 4);

        // act
        var result = session.GetSelectedText();

        // assert
        Assert.AreEqual($"Hello{Environment.NewLine}World{Environment.NewLine}Test", result);
    }

    [TestMethod]
    public void SelectAll_WithSingleLine_SelectsEntireLine()
    {
        // arrange
        var session = CreateSession("Hello");

        // act
        session.SelectAll();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Anchor);
        Assert.AreEqual(new TextPosition(0, 5), session.Caret);
    }

    [TestMethod]
    public void SelectAll_WithMultipleLines_SelectsAllContent()
    {
        // arrange
        var doc = new Doc.Document([new("Hello"), new("World")]);
        var session = new EditorSession(doc);

        // act
        session.SelectAll();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Anchor);
        Assert.AreEqual(new TextPosition(1, 5), session.Caret);
    }

    [TestMethod]
    public void ClearSelection_WithSelection_SetsAnchorToCaret()
    {
        // arrange
        var session = CreateSession("Hello");
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 5);

        // act
        session.ClearSelection();

        // assert
        Assert.AreEqual(session.Caret, session.Anchor);
        Assert.IsFalse(session.HasSelection);
    }
}
