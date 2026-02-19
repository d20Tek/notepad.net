using D20Tek.Notepad.ViewModel;
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace D20Tek.Notepad.Tui;

internal static class EditorViewFactory
{
    public static EditorView Create(EditorViewModel viewModel) => new(viewModel)
    {
        X = 0,
        Y = 0,
        Width = Dim.Fill(),
        Height = Dim.Fill(),
        ColorScheme = new ColorScheme
        {
            Normal = new Attribute(Color.White, Color.Black),
            Focus = new Attribute(Color.White, Color.Blue),
            HotNormal = new Attribute(Color.BrightYellow, Color.Black),
            HotFocus = new Attribute(Color.BrightYellow, Color.White),
            Disabled = new Attribute(Color.Gray, Color.Black)
        }
    };

}
