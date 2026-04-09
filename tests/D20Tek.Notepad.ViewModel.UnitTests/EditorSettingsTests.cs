namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorSettingsTests
{
    [TestMethod]
    public void Default_ReturnsDefaultSettings()
    {
        // act
        var settings = EditorSettings.Default;

        // assert
        Assert.AreEqual(3, settings.MouseWheelScrollLines);
        Assert.AreEqual(3, settings.EdgeScrollSpeed);
        Assert.AreEqual(20, settings.HorizontalEdgeScrollAmount);
        Assert.AreEqual(4, settings.TabSize);
        Assert.IsTrue(settings.UseSpacesForTab);
        Assert.IsFalse(settings.WordWrapEnabled);
        Assert.IsFalse(settings.LineNumbersEnabled);
        Assert.IsEmpty(settings.RecentFiles);
        Assert.AreEqual(10_485_760L, settings.LargeFileThresholdBytes);
    }


    [TestMethod]
    public void Default_ReturnsSameInstance()
    {
        // act
        var settings1 = EditorSettings.Default;
        var settings2 = EditorSettings.Default;

        // assert
        Assert.AreSame(settings1, settings2);
    }

    [TestMethod]
    public void Constructor_WithCustomValues_SetsProperties()
    {
        // act
        var recentFiles = new List<string> { @"C:\files\a.txt", @"C:\files\b.txt" };
        var settings = new EditorSettings
        {
            MouseWheelScrollLines = 5,
            EdgeScrollSpeed = 2,
            HorizontalEdgeScrollAmount = 10,
            TabSize = 2,
            UseSpacesForTab = true,
            WordWrapEnabled = true,
            LineNumbersEnabled = true,
            RecentFiles = recentFiles
        };

        // assert
        Assert.AreEqual(5, settings.MouseWheelScrollLines);
        Assert.AreEqual(2, settings.EdgeScrollSpeed);
        Assert.AreEqual(10, settings.HorizontalEdgeScrollAmount);
        Assert.AreEqual(2, settings.TabSize);
        Assert.IsTrue(settings.UseSpacesForTab);
        Assert.IsTrue(settings.WordWrapEnabled);
        Assert.IsTrue(settings.LineNumbersEnabled);
        Assert.AreEqual(recentFiles, settings.RecentFiles);
    }

    [TestMethod]
    public void RecordEquality_WithSameValues_AreEqual()
    {
        // arrange
        var settings1 = new EditorSettings { MouseWheelScrollLines = 5 };
        var settings2 = new EditorSettings { MouseWheelScrollLines = 5 };

        // assert
        Assert.AreEqual(settings1, settings2);
    }

    [TestMethod]
    public void RecordEquality_WithDifferentValues_AreNotEqual()
    {
        // arrange
        var settings1 = new EditorSettings { MouseWheelScrollLines = 5 };
        var settings2 = new EditorSettings { MouseWheelScrollLines = 3 };

        // assert
        Assert.AreNotEqual(settings1, settings2);
    }

    [TestMethod]
    public void With_CreatesModifiedCopy()
    {
        // arrange
        var original = EditorSettings.Default;

        var recentFiles = new List<string> { @"C:\files\a.txt" };

        // act
        var modified = original with { MouseWheelScrollLines = 10, LineNumbersEnabled = true, RecentFiles = recentFiles };

        // assert
        Assert.AreEqual(3, original.MouseWheelScrollLines);
        Assert.AreEqual(10, modified.MouseWheelScrollLines);
        Assert.AreEqual(original.EdgeScrollSpeed, modified.EdgeScrollSpeed);
        Assert.IsFalse(original.LineNumbersEnabled);
        Assert.IsTrue(modified.LineNumbersEnabled);
        Assert.IsEmpty(original.RecentFiles);
        Assert.AreEqual(recentFiles, modified.RecentFiles);
    }

    [TestMethod]
    public void RecordEquality_WithDifferentLineNumbersEnabled_AreNotEqual()
    {
        // arrange
        var settings1 = new EditorSettings { LineNumbersEnabled = true };
        var settings2 = new EditorSettings { LineNumbersEnabled = false };

        // assert
        Assert.AreNotEqual(settings1, settings2);
    }

    [TestMethod]
    public void Default_LargeFileThresholdBytes_Is10MB()
    {
        // act
        var settings = EditorSettings.Default;

        // assert
        Assert.AreEqual(10_485_760L, settings.LargeFileThresholdBytes);
    }

    [TestMethod]
    public void RecordEquality_WithDifferentLargeFileThresholdBytes_AreNotEqual()
    {
        // arrange
        var settings1 = new EditorSettings { LargeFileThresholdBytes = 10_485_760 };
        var settings2 = new EditorSettings { LargeFileThresholdBytes = 5_242_880 };

        // assert
        Assert.AreNotEqual(settings1, settings2);
    }
}
