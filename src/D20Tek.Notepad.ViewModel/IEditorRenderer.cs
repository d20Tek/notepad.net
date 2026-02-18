namespace D20Tek.Notepad.ViewModel.Rendering;

public interface IEditorRenderer
{
    void BeginFrame(EditorViewModel viewModel);

    void RenderLine(int viewLineIndex, ViewLine line, SelectionSegment? selection);

    void RenderCaret(ViewPosition caret);

    void EndFrame();
}
