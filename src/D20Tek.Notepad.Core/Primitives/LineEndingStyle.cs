namespace D20Tek.Notepad.Core.Primitives;

public enum LineEndingStyle
{
    CRLF,
    LF,
    CR,
    Unknown
}

internal static class LineEndingStyleExtensions
{
    public static string ToLineEndingString(this LineEndingStyle style) => style switch
    {
        LineEndingStyle.CRLF => "\r\n",
        LineEndingStyle.LF => "\n",
        LineEndingStyle.CR => "\r",
        _ => string.Empty
    };

    public static LineEndingStyle FromString(string text) => text switch
    {
        _ when text.Contains("\r\n", StringComparison.Ordinal) => LineEndingStyle.CRLF,
        _ when text.Contains('\n') => LineEndingStyle.LF,
        _ when text.Contains('\r') => LineEndingStyle.CR,
        _ => LineEndingStyle.Unknown
    };
}