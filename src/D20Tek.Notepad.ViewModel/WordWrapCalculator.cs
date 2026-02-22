namespace D20Tek.Notepad.ViewModel;

internal static class WordWrapCalculator
{
    public static List<WrappedSegment> WrapLine(string text, int viewportWidth)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(viewportWidth);

        var segments = new List<WrappedSegment>();
        int currentIndex = 0;

        while (currentIndex < text.Length)
        {
            int remaining = text.Length - currentIndex;
            if (remaining <= viewportWidth)
            {
                segments.Add(new WrappedSegment(currentIndex, remaining, text[currentIndex..]));
                break;
            }

            int breakIndex = FindWordBreakIndex(text, currentIndex, viewportWidth);
            segments.Add(new WrappedSegment(currentIndex, breakIndex - currentIndex, text[currentIndex..breakIndex]));
            currentIndex = SkipLeadingSpaces(text, breakIndex);
        }

        return (segments.Count == 0) ? [new WrappedSegment(0, 0, string.Empty)] : segments;
    }

    private static int FindWordBreakIndex(string text, int startIndex, int viewportWidth)
    {
        int endIndex = startIndex + viewportWidth;

        // Find the last space within the viewport, but we want to break at the first space
        // after the last complete word that fits
        int lastSpaceIndex = text.LastIndexOf(' ', endIndex - 1, viewportWidth);
        if (lastSpaceIndex <= startIndex)
        {
            return endIndex;
        }

        // Walk back to find the first space in a sequence of spaces (trim trailing spaces from segment)
        while (lastSpaceIndex > startIndex && text[lastSpaceIndex - 1] == ' ')
        {
            lastSpaceIndex--;
        }

        return lastSpaceIndex;
    }

    private static int SkipLeadingSpaces(string text, int index)
    {
        var span = text.AsSpan(index);
        int offset = span.IndexOfAnyExcept(' ');
        return offset < 0 ? text.Length : index + offset;
    }
}
