namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class MruListTests
{
    [TestMethod]
    public void Constructor_DefaultPaths_IsEmpty()
    {
        // act
        var mru = new MruList();

        // assert
        Assert.IsEmpty(mru.Paths);
    }

    [TestMethod]
    public void Empty_HasNoPaths()
    {
        // act
        var mru = MruList.Empty;

        // assert
        Assert.IsEmpty(mru.Paths);
    }

    [TestMethod]
    public void Add_SinglePath_PathIsFirst()
    {
        // arrange
        var mru = MruList.Empty;

        // act
        var result = mru.Add(@"C:\files\a.txt");

        // assert
        Assert.AreEqual(@"C:\files\a.txt", result.Paths[0]);
        Assert.HasCount(1, result.Paths);
    }

    [TestMethod]
    public void Add_DuplicatePath_MovesToFront()
    {
        // arrange
        var mru = MruList.Empty.Add(@"C:\files\a.txt").Add(@"C:\files\b.txt");

        // act
        var result = mru.Add(@"C:\files\a.txt");

        // assert
        Assert.AreEqual(@"C:\files\a.txt", result.Paths[0]);
        Assert.AreEqual(@"C:\files\b.txt", result.Paths[1]);
    }

    [TestMethod]
    public void Add_DuplicatePath_DoesNotIncrementCount()
    {
        // arrange
        var mru = MruList.Empty.Add(@"C:\files\a.txt").Add(@"C:\files\b.txt");

        // act
        var result = mru.Add(@"C:\files\a.txt");

        // assert
        Assert.HasCount(2, result.Paths);
    }

    [TestMethod]
    public void Add_BeyondCapacity_OldestEntryDropped()
    {
        // arrange
        var mru = MruList.Empty;
        for (int i = 1; i <= MruList.MaxCapacity; i++)
        {
            mru = mru.Add($@"C:\files\file{i}.txt");
        }

        // act
        var result = mru.Add(@"C:\files\new.txt");

        // assert
        Assert.IsFalse(result.Paths.Contains($@"C:\files\file1.txt"));
    }

    [TestMethod]
    public void Add_BeyondCapacity_CountRemainsAtMax()
    {
        // arrange
        var mru = MruList.Empty;
        for (int i = 1; i <= MruList.MaxCapacity; i++)
        {
            mru = mru.Add($@"C:\files\file{i}.txt");
        }

        // act
        var result = mru.Add(@"C:\files\new.txt");

        // assert
        Assert.HasCount(MruList.MaxCapacity, result.Paths);
    }

    [TestMethod]
    public void Add_NullPath_ThrowsArgumentNullException()
    {
        // arrange
        var mru = MruList.Empty;

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => mru.Add(null!));
    }

    [TestMethod]
    public void Add_WhiteSpacePath_ThrowsArgumentException()
    {
        // arrange
        var mru = MruList.Empty;

        // act & assert
        Assert.ThrowsExactly<ArgumentException>(
            [ExcludeFromCodeCoverage] () => mru.Add("   "));
    }

    [TestMethod]
    public void Update_WithPaths_ReturnsNewInstance()
    {
        // arrange
        var mru = MruList.Empty.Add(@"C:\files\a.txt");

        // act
        var result = mru with { Paths = [@"C:\files\b.txt"] };

        // assert
        Assert.Contains(@"C:\files\b.txt", result.Paths);
    }
}
