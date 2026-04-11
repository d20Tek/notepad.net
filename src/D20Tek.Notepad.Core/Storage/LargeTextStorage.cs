using D20Tek.Notepad.Core.Document;
using D20Tek.Notepad.Core.Primitives;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace D20Tek.Notepad.Core.Storage;

internal sealed class LargeTextStorage
{
    private const long ProgressIntervalMs = 100;

    private readonly int _chunkSize;

    internal LargeTextStorage(int chunkSize = 65536)
    {
        _chunkSize = chunkSize;
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

        var (encoding, preambleLength) = DetectEncoding(accessor, fileLength);
        return LoadSinglePass(accessor, fileLength, encoding, preambleLength, progress, cancellation);
    }

    private static (Encoding Encoding, int PreambleLength) DetectEncoding(
        MemoryMappedViewAccessor accessor,
        long fileLength)
    {
        Span<byte> bom = stackalloc byte[4];
        int toRead = (int)Math.Min(4, fileLength);
        for (int i = 0; i < toRead; i++)
            bom[i] = accessor.ReadByte(i);

        return (bom[0], bom[1], bom[2], bom[3]) switch
        {
            (0xEF, 0xBB, 0xBF, _) => (Encoding.UTF8, 3),
            (0xFF, 0xFE, _, _) => (Encoding.Unicode, 2),
            (0xFE, 0xFF, _, _) => (Encoding.BigEndianUnicode, 2),
            _ => (new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), 0)
        };
    }

    private unsafe DocumentData LoadSinglePass(
        MemoryMappedViewAccessor accessor,
        long fileLength,
        Encoding encoding,
        int preambleLength,
        ILoadProgress? progress,
        CancellationToken cancellation)
    {
        var lines = new List<TextLine>();
        var lineEndingStyle = LineEndingStyle.Unknown;
        var buffer = new byte[_chunkSize];
        var progressTimer = Stopwatch.StartNew();

        byte* basePtr = null;
        accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref basePtr);
        try
        {
            long position = preambleLength;
            long lineStart = preambleLength;
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

                        long contentEnd = prevWasCr ? absolutePos - 1 : absolutePos;
                        EmitLine(lines, basePtr, lineStart, contentEnd, encoding);
                        lineStart = absolutePos + 1;
                        prevWasCr = false;
                    }
                    else if (b == '\r')
                    {
                        if (lineEndingStyle == LineEndingStyle.Unknown)
                        {
                            bool nextIsLf = (i + 1 < toRead && buffer[i + 1] == '\n') ||
                                (i + 1 == toRead && absolutePos + 1 < fileLength &&
                                    accessor.ReadByte(absolutePos + 1) == '\n');
                            lineEndingStyle = nextIsLf ? LineEndingStyle.CRLF : LineEndingStyle.CR;
                        }

                        if (prevWasCr)
                        {
                            EmitLine(lines, basePtr, lineStart, absolutePos - 1, encoding);
                            lineStart = absolutePos;
                        }

                        prevWasCr = true;
                    }
                    else
                    {
                        if (prevWasCr)
                        {
                            EmitLine(lines, basePtr, lineStart, absolutePos - 1, encoding);
                            lineStart = absolutePos;
                        }

                        prevWasCr = false;
                    }
                }

                position += toRead;

                if (progress is not null && progressTimer.ElapsedMilliseconds >= ProgressIntervalMs)
                {
                    progress.Report(position, fileLength);
                    progressTimer.Restart();
                }
            }

            if (prevWasCr)
            {
                EmitLine(lines, basePtr, lineStart, fileLength - 1, encoding);
                lineStart = fileLength;
            }

            if (lineStart < fileLength)
            {
                EmitLine(lines, basePtr, lineStart, fileLength, encoding);
            }
            else if (lineStart == fileLength && lines.Count > 0)
            {
                // file ended exactly on a newline — no trailing empty line to add
            }
            else if (lines.Count == 0)
            {
                lines.Add(TextLine.Empty);
            }
        }
        finally
        {
            accessor.SafeMemoryMappedViewHandle.ReleasePointer();
        }

        progress?.Report(fileLength, fileLength);
        return new DocumentData(lines, encoding, lineEndingStyle);
    }

    private static unsafe void EmitLine(List<TextLine> lines, byte* basePtr, long start, long contentEnd, Encoding encoding)
    {
        int byteCount = (int)(contentEnd - start);
        string content = byteCount <= 0
            ? string.Empty
            : encoding.GetString(new ReadOnlySpan<byte>(basePtr + start, byteCount));

        lines.Add(new TextLine(content));
    }
}
