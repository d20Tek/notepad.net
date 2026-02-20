using D20Tek.Notepad.ViewModel.Rendering;
using Attribute = Terminal.Gui.Attribute;

namespace D20Tek.Notepad.Tui;

public sealed class TerminalGuiRenderer(View target) : IEditorRenderer
{
    private readonly View _target = target;
    private Attribute _normalAttribute;
    private Attribute _selectionAttribute;

    private int AbsX => _target.Frame.X;
    private int AbsY => _target.Frame.Y;
    private int Width => _target.Frame.Width;
    private int Height => _target.Frame.Height;

    // ---------------------------------------------------------
    // Clear the viewport
    // ---------------------------------------------------------
    public void BeginFrame(EditorViewModel viewModel)
    {
        _normalAttribute = _target.ColorScheme.Normal;
        _selectionAttribute = new Attribute(_normalAttribute.Foreground, _target.ColorScheme.Focus.Background);

        var driver = Application.Driver;

        for (int y = 0; y < Height; y++)
        {
            driver.Move(AbsX, AbsY + y);
            driver.SetAttribute(_normalAttribute);
            driver.AddStr(new string(' ', Width));
        }
    }

    // ---------------------------------------------------------
    // Draw a single line with selection highlight
    // ---------------------------------------------------------
    public void RenderLine(int viewLineIndex, ViewLine line, SelectionSegment? selection)
    {
        var driver = Application.Driver;

        driver.Move(AbsX, AbsY + viewLineIndex);

        string text = line.Text ?? string.Empty;

        // Clip text to viewport width
        if (text.Length > Width) text = text[..Width];

        // No selection → draw normally
        if (selection is null)
        {
            driver.SetAttribute(_normalAttribute);
            driver.AddStr(text);
            return;
        }

        int selStart = selection.Value.StartColumn;
        int selEnd = selection.Value.EndColumn;

        // Clamp to visible text
        selStart = Math.Max(0, Math.Min(selStart, text.Length));
        selEnd = Math.Max(0, Math.Min(selEnd, text.Length));

        // Part 1: before selection
        if (selStart > 0)
        {
            driver.SetAttribute(_normalAttribute);
            driver.AddStr(text.Substring(0, selStart));
        }

        // Part 2: selected text
        if (selEnd > selStart)
        {
            driver.SetAttribute(_selectionAttribute);
            driver.AddStr(text[selStart..selEnd]);
        }

        // Part 3: after selection
        if (selEnd < text.Length)
        {
            driver.SetAttribute(_normalAttribute);
            driver.AddStr(text[selEnd..]);
        }
    }

    // ---------------------------------------------------------
    // Draw caret
    // ---------------------------------------------------------
    public void RenderCaret(ViewPosition caret)
    {
        var driver = Application.Driver;

        int x = AbsX + caret.Column;
        int y = AbsY + caret.LineIndex;

        if (x < AbsX || x >= AbsX + Width) return;
        if (y < AbsY || y >= AbsY + Height) return;

        driver.Move(x, y);
        driver.SetCursorVisibility(CursorVisibility.Default);
    }

    public void EndFrame() { }
}
