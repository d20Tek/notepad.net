using Attribute = Terminal.Gui.Attribute;

namespace D20Tek.Notepad.Tui;

internal static class EditorColorSchemes
{
    public static readonly ColorScheme Editor = new()
    {
        Normal = new Attribute(Color.Gray, Color.Black),
        Focus = new Attribute(Color.Gray, Color.Black),
        HotNormal = new Attribute(Color.BrightYellow, Color.Black),
        HotFocus = new Attribute(Color.BrightYellow, Color.Black),
        Disabled = new Attribute(Color.DarkGray, Color.Black)
    };

    public static readonly ColorScheme MenuBar = new()
    {
        Normal = new Attribute(Color.White, Color.Black),
        Focus = new Attribute(Color.Black, Color.Gray),
        HotNormal = new Attribute(Color.BrightBlue, Color.Black),
        HotFocus = new Attribute(Color.BrightBlue, Color.DarkGray),
        Disabled = new Attribute(Color.DarkGray, Color.Black)
    };

    public static readonly ColorScheme Divider = new()
    {
        Normal = new Attribute(Color.DarkGray, Color.Black)
    };

    public static readonly ColorScheme StatusBar = new()
    {
        Normal = new Attribute(Color.Black, Color.DarkGray),
        Focus = new Attribute(Color.Black, Color.DarkGray),
        HotNormal = new Attribute(Color.Black, Color.DarkGray),
        HotFocus = new Attribute(Color.Black, Color.DarkGray),
        Disabled = new Attribute(Color.DarkGray, Color.DarkGray)
    };

    public static readonly Attribute GutterNormal = new(Color.DarkGray, Color.Black);

    public static readonly Attribute GutterActive = new(Color.Gray, Color.Black);
}
