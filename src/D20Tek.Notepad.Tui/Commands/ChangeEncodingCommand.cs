using System.Text;

namespace D20Tek.Notepad.Tui.Commands;

internal static class ChangeEncodingCommand
{
    public const string Utf8CommandName = "EncodingUtf8";
    public const string Utf8BomCommandName = "EncodingUtf8Bom";
    public const string UnicodeCommandName = "EncodingUnicode";
    public const string BigEndianUnicodeCommandName = "EncodingBigEndianUnicode";

    public static UiCommand CreateUtf8(EditorViewModel viewModel) =>
        new(Utf8CommandName, () => viewModel.ChangeEncoding(new UTF8Encoding(false)));

    public static UiCommand CreateUtf8Bom(EditorViewModel viewModel) =>
        new(Utf8BomCommandName, () => viewModel.ChangeEncoding(new UTF8Encoding(true)));

    public static UiCommand CreateUnicode(EditorViewModel viewModel) =>
        new(UnicodeCommandName, () => viewModel.ChangeEncoding(Encoding.Unicode));

    public static UiCommand CreateBigEndianUnicode(EditorViewModel viewModel) =>
        new(BigEndianUnicodeCommandName, () => viewModel.ChangeEncoding(Encoding.BigEndianUnicode));
}
