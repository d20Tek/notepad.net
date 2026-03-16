namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class StatusDetailsTests
{
    [TestMethod]
    public void Constructor_WithValidParameters_SetsProperties()
    {
        // arrange / act
        var status = new StatusDetails(
            CurrentLine: 5,
            CurrentColumn: 12,
            TotalLines: 100,
            TotalCharacters: 4521,
            DocumentEncoding: "UTF-8",
            LineEndingStyle: "CRLF",
            IsOverwriteMode: false);

        // assert
        Assert.AreEqual(5, status.CurrentLine);
        Assert.AreEqual(12, status.CurrentColumn);
        Assert.AreEqual(100, status.TotalLines);
        Assert.AreEqual(4521, status.TotalCharacters);
        Assert.AreEqual("UTF-8", status.DocumentEncoding);
        Assert.AreEqual("CRLF", status.LineEndingStyle);
        Assert.IsFalse(status.IsOverwriteMode);
    }

    [TestMethod]
    public void With_ChangingAllProperties_CreatesNewInstanceWithAllUpdates()
    {
        // arrange
        var original = new StatusDetails(1, 1, 1, 0, "UTF-8", "CRLF");

        // act
        var modified = original with
        {
            CurrentLine = 10,
            CurrentColumn = 5,
            TotalLines = 50,
            TotalCharacters = 200,
            DocumentEncoding = "US-ASCII",
            LineEndingStyle = "LF",
            IsOverwriteMode = true
        };

        // assert
        Assert.AreNotSame(original, modified);
        Assert.AreEqual(10, modified.CurrentLine);
        Assert.AreEqual(5, modified.CurrentColumn);
        Assert.AreEqual(50, modified.TotalLines);
        Assert.AreEqual(200, modified.TotalCharacters);
        Assert.AreEqual("US-ASCII", modified.DocumentEncoding);
        Assert.AreEqual("LF", modified.LineEndingStyle);
        Assert.IsFalse(original.IsOverwriteMode);
        Assert.IsTrue(modified.IsOverwriteMode);
    }

    [TestMethod]
    public void Equality_SameValues_AreEqual()
    {
        // arrange
        var status1 = new StatusDetails(3, 7, 10, 500, "UTF-8", "CRLF");
        var status2 = new StatusDetails(3, 7, 10, 500, "UTF-8", "CRLF");

        // assert
        Assert.AreEqual(status1, status2);
        Assert.IsTrue(status1 == status2);
    }

    [TestMethod]
    public void Equality_DifferentValues_AreNotEqual()
    {
        // arrange
        var status1 = new StatusDetails(3, 7, 10, 500, "UTF-8", "CRLF");
        var status2 = new StatusDetails(3, 7, 10, 500, "UTF-8", "CRLF", IsOverwriteMode: true);

        // assert
        Assert.AreNotEqual(status1, status2);
        Assert.IsTrue(status1 != status2);
    }

    [TestMethod]
    public void Empty_HasSensibleDefaults()
    {
        // act
        var status = StatusDetails.Empty;

        // assert
        Assert.AreEqual(1, status.CurrentLine);
        Assert.AreEqual(1, status.CurrentColumn);
        Assert.AreEqual(1, status.TotalLines);
        Assert.AreEqual(0, status.TotalCharacters);
        Assert.AreEqual("UTF-8", status.DocumentEncoding);
        Assert.AreEqual("CRLF", status.LineEndingStyle);
        Assert.IsFalse(status.IsOverwriteMode);
    }
}
