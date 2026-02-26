using D20Tek.Notepad.Core.Search;

namespace D20Tek.Notepad.Core.UnitTests.Search;

[TestClass]
public class SearchOptionsTests
{
    [TestMethod]
    public void Constructor_SetsProperties()
    {
        // arrange & act
        var options = new SearchOptions
        {
            CaseSensitive = true,
            WrapAround = false
        };

        // assert
        Assert.IsTrue(options.CaseSensitive);
        Assert.IsFalse(options.WrapAround);
    }

    [TestMethod]
    public void With_ChangesAllProperties()
    {
        // arrange
        var original = new SearchOptions { CaseSensitive = false, WrapAround = true };

        // act
        var modified = original with { CaseSensitive = true, WrapAround = false };

        // assert
        Assert.AreNotSame(original, modified);
        Assert.IsTrue(modified.CaseSensitive);
        Assert.IsFalse(modified.WrapAround);
        Assert.IsFalse(original.CaseSensitive);
        Assert.IsTrue(original.WrapAround);
    }

    [TestMethod]
    public void Equality_SameValues_AreEqual()
    {
        // arrange
        var options1 = new SearchOptions { CaseSensitive = true, WrapAround = false };
        var options2 = new SearchOptions { CaseSensitive = true, WrapAround = false };

        // act & assert
        Assert.AreEqual(options1, options2);
    }

    [TestMethod]
    public void Equality_DifferentValues_AreNotEqual()
    {
        // arrange
        var options1 = new SearchOptions { CaseSensitive = true, WrapAround = true };
        var options2 = new SearchOptions { CaseSensitive = false, WrapAround = true };

        // act & assert
        Assert.AreNotEqual(options1, options2);
    }

    [TestMethod]
    public void Default_HasExpectedValues()
    {
        // arrange & act
        var options = SearchOptions.Default;

        // assert
        Assert.IsFalse(options.CaseSensitive);
        Assert.IsTrue(options.WrapAround);
    }
}
