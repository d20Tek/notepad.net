using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.Core.Document;

internal static class DocumentPositionExtensions
{
    public static TextPosition Normalize(this IDocument doc, TextPosition pos)
    {
        // clamp line
        int line = Math.Clamp(pos.Line, 0, doc.Lines.Count - 1);

        // clamp column based on line length
        int maxColumn = doc.Lines[line].Content.Length;
        int column = Math.Clamp(pos.Column, 0, maxColumn);

        return new TextPosition(line, column);
    }
}
