using D20Tek.Notepad.Core.Document;
using D20Tek.Notepad.Core.Primitives;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace D20Tek.Notepad.Core.Storage;

internal sealed class LargeTextStorage
{
    private readonly int _chunkSize;
    private readonly int _progressInterval;

    internal LargeTextStorage(int chunkSize = 65536, int progressInterval = 4096)
    {
        _chunkSize = chunkSize;
        _progressInterval = progressInterval;
    }

    public DocumentData Load(string filePath, ILoadProgress? progress = null, CancellationToken cancellation = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var fileInfo = new FileInfo(filePath);
        long fileLength = fileInfo.Length;

        if (fileLength == 0)
            return new DocumentData(
                [TextLine.Empty],
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
                LineEndingStyle.Unknown);

        using var mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
        using var accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);

        var (encoding, preambleLength) = DetectEncoding(accessor);
        var (lineOffsets, lineEndingStyle) = BuildLineIndex(accessor, fileLength, preambleLength, progress, cancellation);
        var lines = MaterializeLines(accessor, lineOffsets, fileLength, encoding, progress, cancellation);

        return new DocumentData(lines, encoding, lineEndingStyle);
    }

    private static (Encoding Encoding, int PreambleLength) DetectEncoding(MemoryMappedViewAccessor accessor)
    {
        byte b0 = accessor.ReadByte(0);
        byte b1 = accessor.ReadByte(1);
        byte b2 = accessor.ReadByte(2);
        byte b3 = accessor.ReadByte(3);

        return (b0, b1, b2, b3) switch
        {
            (0xEF, 0xBB, 0xBF, _) => (Encoding.UTF8, 3),
            (0xFF, 0xFE, _, _) => (Encoding.Unicode, 2),
            (0xFE, 0xFF, _, _) => (Encoding.BigEndianUnicode, 2),
            _ => (new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), 0)
        };
    }

    private (List<long> Offsets, LineEndingStyle Style) BuildLineIndex(
        MemoryMappedViewAccessor accessor,
        long fileLength,
        int preambleLength,
        ILoadProgress? progress,
        CancellationToken cancellation)
    {
        var offsets = new List<long> { preambleLength };

        var lineEndingStyle = LineEndingStyle.Unknown;
        var buffer = new byte[_chunkSize];
        long position = preambleLength;
        int reportCounter = 0;
        bool prevWasCr = false;

        while (position < fileLength)
        {
            cancellation.ThrowIfCancellationRequested();

            long remaining = fileLength - position;
            int toRead = (int)Math.Min(_chunkSize, remaining);
            accessor.ReadArray(position, buffer, 0, toRead);

            for (int i = 0; i < toRead; i++)
            {
                byte b = buffer[i];
                long absolutePos = position + i;

                if (b == '\n')
                {
                    if (lineEndingStyle == LineEndingStyle.Unknown)
                        lineEndingStyle = prevWasCr ? LineEndingStyle.CRLF : LineEndingStyle.LF;

                    offsets.Add(absolutePos + 1);
                    prevWasCr = false;
                }
                else if (b == '\r')
                {
                    if (lineEndingStyle == LineEndingStyle.Unknown)
                    {
                        bool nextIsCr = (i + 1 < toRead && buffer[i + 1] == '\n') ||
                                        (i + 1 == toRead && absolutePos + 1 < fileLength && accessor.ReadByte(absolutePos + 1) == '\n');
                        lineEndingStyle = nextIsCr ? LineEndingStyle.CRLF : LineEndingStyle.CR;
                    }

                    if (!prevWasCr)
                    {
                        bool nextIsLf = (i + 1 < toRead && buffer[i + 1] == '\n') ||
                                        (i + 1 == toRead && absolutePos + 1 < fileLength && accessor.ReadByte(absolutePos + 1) == '\n');
                        if (!nextIsLf)
                            offsets.Add(absolutePos + 1);
                    }

                    prevWasCr = true;
                }
                else
                {
                    prevWasCr = false;
                }
            }

            position += toRead;
            reportCounter++;

            if (reportCounter >= _progressInterval)
            {
                if (progress is not null)
                {
                    progress.Report(position, fileLength);
                }
                reportCounter = 0;
            }
        }

        progress?.Report(fileLength, fileLength);

        // Remove trailing empty offset if file ended exactly on newline
        if (offsets.Count > 1 && offsets[offsets.Count - 1] >= fileLength)
            offsets.RemoveAt(offsets.Count - 1);

        return (offsets, lineEndingStyle);
    }

    private List<TextLine> MaterializeLines(
        MemoryMappedViewAccessor accessor,
        List<long> lineOffsets,
        long fileLength,
        Encoding encoding,
        ILoadProgress? progress,
        CancellationToken cancellation)
    {
        var lines = new List<TextLine>(lineOffsets.Count);

        for (int i = 0; i < lineOffsets.Count; i++)
        {
            if (i % _progressInterval == 0)
            {
                cancellation.ThrowIfCancellationRequested();
                progress?.Report(lineOffsets[i], fileLength);
            }

            long start = lineOffsets[i];
            long end = (i + 1 < lineOffsets.Count) ? lineOffsets[i + 1] : fileLength;

            // Strip line ending bytes from end
            long contentEnd = end;
            if (contentEnd > start)
            {
                byte lastByte = accessor.ReadByte(contentEnd - 1);
                if (lastByte == '\n')
                {
                    contentEnd--;
                    if (contentEnd > start && accessor.ReadByte(contentEnd - 1) == '\r')
                        contentEnd--;
                }
                else if (lastByte == '\r')
                {
                    contentEnd--;
                }
            }

            int byteCount = (int)(contentEnd - start);
            string content;

            if (byteCount <= 0)
            {
                content = string.Empty;
            }
            else
            {
                var bytes = new byte[byteCount];
                accessor.ReadArray(start, bytes, 0, byteCount);
                content = encoding.GetString(bytes);
            }

            lines.Add(new TextLine(content));
        }

        progress?.Report(fileLength, fileLength);
        return lines;
    }
}
