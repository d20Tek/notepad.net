using System.Text;

namespace D20Tek.Notepad.Core.UnitTests.Storage;

internal static class MemoryStreamFactory
{
    public static MemoryStream CreateStream(string content)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream, new UTF8Encoding(false));
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    public static MemoryStream CreateStreamWithBom(string content, Encoding encoding)
    {
        var stream = new MemoryStream();
        var preamble = encoding.GetPreamble();
        stream.Write(preamble);
        var bytes = encoding.GetBytes(content);
        stream.Write(bytes);
        stream.Position = 0;
        return stream;
    }

    public static string ReadStream(MemoryStream stream)
    {
        stream.Position = 0;
        using var reader = new StreamReader(stream, leaveOpen: true);
        return reader.ReadToEnd();
    }
}
