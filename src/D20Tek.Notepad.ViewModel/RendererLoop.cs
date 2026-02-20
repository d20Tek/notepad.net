using D20Tek.Notepad.ViewModel.Rendering;

namespace D20Tek.Notepad.ViewModel;

internal static class RendererLoop
{
    public static void RenderFull(EditorViewModel viewModel, IEditorRenderer renderer)
    {
        renderer.BeginFrame(viewModel);

        for (int i = 0; i < viewModel.VisibleLines.Count; i++)
        {
            var line = viewModel.VisibleLines[i];
            var segment = viewModel.GetSelectionSegmentForLine(i);

            renderer.RenderLine(i, line, segment);
        }

        renderer.RenderCaret(viewModel.CaretViewPosition);

        renderer.EndFrame();
    }
}
