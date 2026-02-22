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
        Assert.IsFalse(settings.WordWrapEnabled);
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
        var settings = new EditorSettings
        {
            MouseWheelScrollLines = 5,
            EdgeScrollSpeed = 2,
            HorizontalEdgeScrollAmount = 10,
            TabSize = 2,
            WordWrapEnabled = true
        };

        // assert
        Assert.AreEqual(5, settings.MouseWheelScrollLines);
        Assert.AreEqual(2, settings.EdgeScrollSpeed);
        Assert.AreEqual(10, settings.HorizontalEdgeScrollAmount);
        Assert.AreEqual(2, settings.TabSize);
        Assert.IsTrue(settings.WordWrapEnabled);
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

        // act
        var modified = original with { MouseWheelScrollLines = 10 };

        // assert
        Assert.AreEqual(3, original.MouseWheelScrollLines);
        Assert.AreEqual(10, modified.MouseWheelScrollLines);
        Assert.AreEqual(original.EdgeScrollSpeed, modified.EdgeScrollSpeed);
    }
}
