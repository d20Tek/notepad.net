namespace D20Tek.Notepad.Tui;

internal static class EditorViewFactory
{
    public static EditorView Create(EditorViewModel viewModel, bool statusBarEnabled) => new(viewModel)
    {
        X = 0,
        Y = 1,
        Width = Dim.Fill(),
        Height = statusBarEnabled ? Dim.Fill() - 1 : Dim.Fill(),
        ColorScheme = EditorColorSchemes.Editor
    };
}
