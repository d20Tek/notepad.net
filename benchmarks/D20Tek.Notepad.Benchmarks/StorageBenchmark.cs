using D20Tek.Notepad.Core.Document;
using System.Diagnostics;
using System.Text;

namespace D20Tek.Notepad.Benchmarks;

internal static class StorageBenchmark
{
    private static readonly int[] FileSizesMb = [1, 10, 100];

    public static void Run()
    {
        Console.WriteLine("Storage Benchmark: LargeTextStorage vs SimpleTextStorage");
        Console.WriteLine(new string('=', 64));
        Console.WriteLine();

        foreach (int sizeMb in FileSizesMb)
        {
            RunForSize(sizeMb);
        }
    }

    private static void RunForSize(int sizeMb)
    {
        Console.Write($"Generating {sizeMb,3} MB test file... ");
        string path = GenerateTestFile(sizeMb);
        Console.WriteLine("done.");

        try
        {
            // warm-up pass — JIT compile the load paths without recording results
            _ = new DocumentFactory().Load(path, largeFileThreshold: long.MaxValue);
            GC.Collect(2, GCCollectionMode.Forced, blocking: true);

            MeasureLoad("SimpleTextStorage", path, largeFileThreshold: long.MaxValue);
            GC.Collect(2, GCCollectionMode.Forced, blocking: true);
            MeasureLoad("LargeTextStorage ", path, largeFileThreshold: 0);
        }
        finally
        {
            File.Delete(path);
            Console.WriteLine();
        }
    }

    private static void MeasureLoad(string label, string filePath, long largeFileThreshold)
    {
        long allocBefore = GC.GetTotalAllocatedBytes(precise: false);
        var sw = Stopwatch.StartNew();

        var doc = new DocumentFactory().Load(filePath, largeFileThreshold);

        sw.Stop();
        long allocMb = (GC.GetTotalAllocatedBytes(precise: false) - allocBefore) / 1024 / 1024;

        Console.WriteLine($"  {label}  {sw.ElapsedMilliseconds,6} ms  {doc.Lines.Count,9} lines  {allocMb,4} MB alloc");
    }

    private static string GenerateTestFile(int sizeMb)
    {
        string path = Path.GetTempFileName();
        long targetBytes = (long)sizeMb * 1024 * 1024;

        using var writer = new StreamWriter(path, append: false, encoding: new UTF8Encoding(false));
        long written = 0;
        int lineNum = 0;

        while (written < targetBytes)
        {
            string line =
                $"Line {lineNum++,8}: Sample content for benchmarking large file load performance in .NET 10.";
            writer.WriteLine(line);
            written += line.Length + 1;
        }

        return path;
    }
}
