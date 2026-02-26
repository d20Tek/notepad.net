namespace D20Tek.Notepad.Tui;

internal class HorizontalDivider : View
{
    public HorizontalDivider(int x, int y)
    {
        X = x;
        Y = y;
        Height = 1;
        Width = Dim.Fill();
        ColorScheme = EditorColorSchemes.Divider;
    }

    public override void Redraw(Rect bounds)
    {
        Driver.SetAttribute(ColorScheme.Normal);

        for (int x = 0; x < bounds.Width; x++)
        {
            Move(x, 0);
            Driver.AddRune(Driver.HLine);
        }
    }
}
