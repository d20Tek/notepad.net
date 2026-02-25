using Attribute = Terminal.Gui.Attribute;

namespace D20Tek.Notepad.Tui;

internal static class EditorViewFactory
{
    public static EditorView Create(EditorViewModel viewModel) => new(viewModel)
    {
        X = 0,
        Y = 0, // Will be set by caller
        Width = Dim.Fill(),
        Height = Dim.Fill(),
        ColorScheme = new ColorScheme
        {
            Normal = new Attribute(Color.Gray, Color.Black),
            Focus = new Attribute(Color.Gray, Color.Black),
            HotNormal = new Attribute(Color.BrightYellow, Color.Black),
            HotFocus = new Attribute(Color.BrightYellow, Color.Black),
            Disabled = new Attribute(Color.DarkGray, Color.Black)
        }
    };
}
