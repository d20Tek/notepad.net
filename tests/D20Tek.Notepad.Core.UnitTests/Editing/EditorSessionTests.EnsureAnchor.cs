namespace D20Tek.Notepad.Core.UnitTests.Editing;

public partial class EditorSessionTests
{
    // EnsureAnchorExists tests
    [TestMethod]
    public void EnsureAnchorExists_WhenNoSelection_SetsAnchorToCaret()
    {
        // arrange
        var session = CreateSession("Hello World");
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5); // No selection

        // act
        session.EnsureAnchorExists();

        // assert
        Assert.AreEqual(new TextPosition(0, 5), session.Anchor);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void EnsureAnchorExists_WhenSelectionExists_KeepsExistingAnchor()
    {
        // arrange
        var session = CreateSession("Hello World");
        session.Anchor = new TextPosition(0, 2);
        session.Caret = new TextPosition(0, 8); // Selection exists

        // act
        session.EnsureAnchorExists();

        // assert
        Assert.AreEqual(new TextPosition(0, 2), session.Anchor);
        Assert.AreEqual(new TextPosition(0, 8), session.Caret);
    }

    [TestMethod]
    public void EnsureAnchorExists_CalledMultipleTimes_KeepsOriginalAnchor()
    {
        // arrange
        var session = CreateSession("Hello World");
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        session.EnsureAnchorExists();
        session.Caret = new TextPosition(0, 8);
        session.EnsureAnchorExists();

        // assert
        Assert.AreEqual(new TextPosition(0, 5), session.Anchor);
        Assert.AreEqual(new TextPosition(0, 8), session.Caret);
    }
}