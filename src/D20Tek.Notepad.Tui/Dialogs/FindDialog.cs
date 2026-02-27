using D20Tek.Notepad.Core.Search;

namespace D20Tek.Notepad.Tui.Dialogs;

internal sealed class FindDialog
{
    public enum FindAction
    {
        None,
        FindNext,
        FindPrevious,
        Close
    }

    public string SearchTerm { get; private set; } = string.Empty;

    public SearchOptions Options { get; private set; } = SearchOptions.Default;

    public FindAction Action { get; private set; } = FindAction.None;

    public void Show(string initialSearchTerm, SearchOptions? initialOptions = null)
    {
        var options = initialOptions ?? SearchOptions.Default;
        var dialog = new Dialog("Find", 50, 10);

        var searchLabel = new Label("Find what:") { X = 1, Y = 1 };
        var searchField = new TextField(initialSearchTerm) { X = 12, Y = 1, Width = Dim.Fill() - 2 };
        var caseSensitiveCheck = new CheckBox("Case sensitive") { X = 1, Y = 3, Checked = options.CaseSensitive };
        var wrapAroundCheck = new CheckBox("Wrap around") { X = 1, Y = 4, Checked = options.WrapAround };
        var findNextButton = new Button("Find _Next") { X = 1, Y = 6 };
        var findPrevButton = new Button("Find _Previous") { X = 15, Y = 6 };
        var closeButton = new Button("Close") { X = Pos.Right(findPrevButton) + 2, Y = 6 };

        findNextButton.Clicked += () =>
            OnButtonClick(searchField, caseSensitiveCheck, wrapAroundCheck, FindAction.FindNext);
        findPrevButton.Clicked += () =>
            OnButtonClick(searchField, caseSensitiveCheck, wrapAroundCheck, FindAction.FindPrevious);
        closeButton.Clicked += () => OnButtonClick(searchField, caseSensitiveCheck, wrapAroundCheck, FindAction.Close);

        dialog.Add(searchLabel, searchField, caseSensitiveCheck, wrapAroundCheck);
        dialog.Add(findNextButton, findPrevButton, closeButton);
        searchField.SetFocus();

        Application.Run(dialog);
    }

    private void OnButtonClick(
        TextField searchField, CheckBox caseSensitiveCheck, CheckBox wrapAroundCheck, FindAction action)
    {
        SearchTerm = searchField.Text?.ToString() ?? string.Empty;
        Options = new SearchOptions
        {
            CaseSensitive = caseSensitiveCheck.Checked,
            WrapAround = wrapAroundCheck.Checked
        };

        Action = action;
        Application.RequestStop();
    }

    public static FindDialog Create() => new();
}
