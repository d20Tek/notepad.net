namespace D20Tek.Notepad.Tui.Dialogs;

internal sealed class GoToLineDialog
{
    public int? LineNumber { get; private set; }

    public bool Canceled { get; private set; } = true;

    public void Show(int currentLine, int totalLines)
    {
        var dialog = new Dialog("Go to Line", 40, 8);

        var label = new Label($"Line number (1 - {totalLines}):") { X = 1, Y = 1 };
        var lineField = new TextField(currentLine.ToString()) { X = 1, Y = 2, Width = Dim.Fill() - 2 };
        var goToButton = new Button("Go To") { X = Pos.Center() - 10, Y = 4 };
        var cancelButton = new Button("Cancel") { X = Pos.Center() + 2, Y = 4 };

        goToButton.Clicked += () => OnGotoLine(lineField, totalLines);
        cancelButton.Clicked += OnCancel;

        dialog.Add(label, lineField, goToButton, cancelButton);
        lineField.SetFocus();

        Application.Run(dialog);
    }

    private void OnGotoLine(TextField lineField, int totalLines)
    {
        string text = lineField.Text?.ToString() ?? string.Empty;
        if (int.TryParse(text, out int line) && line >= 1 && line <= totalLines)
        {
            LineNumber = line;
            Canceled = false;
            Application.RequestStop();
        }
        else
        {
            MessageBox.ErrorQuery("Invalid Input", $"Please enter a number between 1 and {totalLines}.", "OK");
        }
    }

    private void OnCancel()
    {
        Canceled = true;
        Application.RequestStop();
    }

    public static GoToLineDialog Create() => new();
}
