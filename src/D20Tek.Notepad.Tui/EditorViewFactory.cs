namespace D20Tek.Notepad.Tui;

internal static class EditorViewFactory
{
    public static EditorView Create(EditorViewModel viewModel) => new(viewModel)
    {
        X = 0,
        Y = 1,
        Width = Dim.Fill(),
        Height = Dim.Fill(),
        ColorScheme = EditorColorSchemes.Editor
    };
}
