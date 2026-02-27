using D20Tek.Notepad.Core.Search;

namespace D20Tek.Notepad.Tui.Dialogs;

internal sealed class ReplaceDialog
{
    public enum ReplaceAction
    {
        None,
        FindNext,
        Replace,
        ReplaceAll,
        Close
    }

    public string SearchTerm { get; private set; } = string.Empty;

    public string ReplacementText { get; private set; } = string.Empty;

    public SearchOptions Options { get; private set; } = SearchOptions.Default;

    public ReplaceAction Action { get; private set; } = ReplaceAction.None;

    public void Show(string initialSearchTerm, string initialReplacement, SearchOptions? initialOptions = null)
    {
        var options = initialOptions ?? SearchOptions.Default;
        var dialog = new Dialog("Replace", 55, 12);

        var searchLabel = new Label("Find what:") { X = 1, Y = 1 };
        var searchField = new TextField(initialSearchTerm) { X = 14, Y = 1, Width = Dim.Fill() - 2 };
        var replaceLabel = new Label("Replace with:") { X = 1, Y = 2 };
        var replaceField = new TextField(initialReplacement) { X = 14, Y = 2, Width = Dim.Fill() - 2 };
        var caseSensitiveCheck = new CheckBox("Case sensitive") { X = 1, Y = 4, Checked = options.CaseSensitive };
        var wrapAroundCheck = new CheckBox("Wrap around") { X = 1, Y = 5, Checked = options.WrapAround };
        var findNextButton = new Button("Find _Next") { X = 1, Y = 7 };
        var replaceButton = new Button("_Replace") { X = 15, Y = 7 };
        var replaceAllButton = new Button("Replace _All") { X = 27, Y = 7 };
        var closeButton = new Button("Close") { X = 43, Y = 7 };

        findNextButton.Clicked += () => OnButtonClick(
            searchField, replaceField, caseSensitiveCheck, wrapAroundCheck, ReplaceAction.FindNext);

        replaceButton.Clicked += () => OnButtonClick(
            searchField, replaceField, caseSensitiveCheck, wrapAroundCheck, ReplaceAction.Replace);

        replaceAllButton.Clicked += () => OnButtonClick(
            searchField, replaceField, caseSensitiveCheck, wrapAroundCheck, ReplaceAction.ReplaceAll);

        closeButton.Clicked += () => OnButtonClick(
            searchField, replaceField, caseSensitiveCheck, wrapAroundCheck, ReplaceAction.Close);

        dialog.Add(searchLabel, searchField, replaceLabel, replaceField);
        dialog.Add(caseSensitiveCheck, wrapAroundCheck);
        dialog.Add(findNextButton, replaceButton, replaceAllButton, closeButton);
        searchField.SetFocus();

        Application.Run(dialog);
    }

    private void OnButtonClick(
        TextField searchField,
        TextField replaceField,
        CheckBox caseSensitiveCheck,
        CheckBox wrapAroundCheck,
        ReplaceAction action)
    {
        SearchTerm = searchField.Text?.ToString() ?? string.Empty;
        ReplacementText = replaceField.Text?.ToString() ?? string.Empty;
        Options = new SearchOptions
        {
            CaseSensitive = caseSensitiveCheck.Checked,
            WrapAround = wrapAroundCheck.Checked
        };

        Action = action;
        Application.RequestStop();
    }

    public static ReplaceDialog Create() => new();
}
