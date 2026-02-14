using D20Tek.Notepad.Core.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace D20Tek.Notepad.Core.UnitTests.Primitives;

[TestClass]
public class TextLineTests
{
    [TestMethod]
    public void Constructor_SetsContent()
    {
        // arrange
        var expected = "line content";

        // act
        var line = new TextLine(expected);

        // assert
        Assert.AreEqual(expected, line.Content);
    }
}
