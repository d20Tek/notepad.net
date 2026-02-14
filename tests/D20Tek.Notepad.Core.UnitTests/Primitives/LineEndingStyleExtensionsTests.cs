using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.Core.UnitTests.Primitives;

[TestClass]
public class LineEndingStyleExtensionsTests
{
    [TestMethod]
    [DataRow(LineEndingStyle.CRLF, "\r\n")]
    [DataRow(LineEndingStyle.LF, "\n")]
    [DataRow(LineEndingStyle.CR, "\r")]
    [DataRow(LineEndingStyle.Unknown, "")]
    public void ToString_AllStyles_ReturnsExpectedValue(LineEndingStyle style, string expected)
    {
        // arrange

        // act
        var result = style.ToLineEndingString();

        // assert
        Assert.AreEqual(expected, result);
    }
}
