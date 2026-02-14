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
}