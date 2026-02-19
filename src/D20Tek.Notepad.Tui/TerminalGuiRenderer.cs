using D20Tek.Notepad.ViewModel;
using D20Tek.Notepad.ViewModel.Rendering;
using Terminal.Gui;

namespace D20Tek.Notepad.Tui;

public sealed class TerminalGuiRenderer(View target) : IEditorRenderer
{
    private readonly View _target = target;

    public void BeginFrame(EditorViewModel viewModel)
    {
        var driver = Application.Driver;

        int absX = _target.Frame.X;
        int absY = _target.Frame.Y;

        for (int y = 0; y < _target.Frame.Height; y++)
        {
            driver.Move(absX, absY + y);
            driver.AddStr(new string(' ', _target.Frame.Width));
        }
    }

    public void RenderLine(int viewLineIndex, ViewLine line, SelectionSegment? selection)
    {
        var driver = Application.Driver;

        int absX = _target.Frame.X;
        int absY = _target.Frame.Y;

        driver.Move(absX, absY + viewLineIndex);

        if (selection is null)
        {
            var visible = line.Text;
            if (visible.Length > _target.Bounds.Width)
                visible = visible[.._target.Bounds.Width];

            driver.AddStr(visible);
            return;
        }

        // selection drawing logic stays the same
    }

    public void RenderCaret(ViewPosition caret)
    {
        var driver = Application.Driver;

        int absX = _target.Frame.X;
        int absY = _target.Frame.Y;

        driver.Move(absX + caret.Column, absY + caret.LineIndex);
        Application.Driver.SetCursorVisibility(CursorVisibility.Default);
    }

    public void EndFrame()
    {
        // Nothing needed yet
    }
}
