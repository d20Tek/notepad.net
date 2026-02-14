using System.Text;

namespace D20Tek.Notepad.Core.Storage;

internal static class EncodingDetector
{
    public record struct EncodingResult(Encoding Encoding, int PreambleLength);

    public static EncodingResult DetectEncoding(this Stream stream)
    {
        Span<byte> bom = stackalloc byte[4];
        int read = stream.Read(bom);

        return bom switch
        {
            [0xEF, 0xBB, 0xBF, ..] => new(Encoding.UTF8, 3),
            [0xFF, 0xFE, ..] => new(Encoding.Unicode, 2),
            [0xFE, 0xFF, ..] => new(Encoding.BigEndianUnicode, 2),
            _ => new(new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), 0)
        };
    }
}
