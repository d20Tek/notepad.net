using D20Tek.Notepad.Core.Storage;
using System.Text;

namespace D20Tek.Notepad.Core.UnitTests.Storage;

[TestClass]
public class EncodingResultTests
{
    [TestMethod]
    public void Constructor_WithValidParameters_SetsProperties()
    {
        // arrange
        var encoding = Encoding.UTF8;
        var preambleLength = 3;

        // act
        var result = new EncodingDetector.EncodingResult(encoding, preambleLength);

        // assert
        Assert.AreEqual(encoding, result.Encoding);
        Assert.AreEqual(preambleLength, result.PreambleLength);
    }

    [TestMethod]
    public void With_ChangingAllProperties_CreatesNewInstanceWithAllUpdates()
    {
        // arrange
        var original = new EncodingDetector.EncodingResult(Encoding.UTF8, 3);

        // act
        var modified = original with
        {
            Encoding = Encoding.Unicode,
            PreambleLength = 2
        };

        // assert
        Assert.AreNotEqual(original, modified);
        Assert.AreEqual(Encoding.Unicode, modified.Encoding);
        Assert.AreEqual(2, modified.PreambleLength);
    }

    [TestMethod]
    public void Equality_SameValues_AreEqual()
    {
        // arrange
        var result1 = new EncodingDetector.EncodingResult(Encoding.UTF8, 3);
        var result2 = new EncodingDetector.EncodingResult(Encoding.UTF8, 3);

        // act
        var areEqual = result1 == result2;

        // assert
        Assert.IsTrue(areEqual);
        Assert.AreEqual(result1, result2);
    }

    [TestMethod]
    public void Equality_DifferentEncoding_AreNotEqual()
    {
        // arrange
        var result1 = new EncodingDetector.EncodingResult(Encoding.UTF8, 3);
        var result2 = new EncodingDetector.EncodingResult(Encoding.Unicode, 3);

        // act
        var areNotEqual = result1 != result2;

        // assert
        Assert.IsTrue(areNotEqual);
        Assert.AreNotEqual(result1, result2);
    }

    [TestMethod]
    public void Equality_DifferentPreambleLength_AreNotEqual()
    {
        // arrange
        var result1 = new EncodingDetector.EncodingResult(Encoding.UTF8, 3);
        var result2 = new EncodingDetector.EncodingResult(Encoding.UTF8, 0);

        // act
        var areNotEqual = result1 != result2;

        // assert
        Assert.IsTrue(areNotEqual);
        Assert.AreNotEqual(result1, result2);
    }
}