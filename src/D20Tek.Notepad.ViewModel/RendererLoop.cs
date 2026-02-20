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

    // todo: remove if unnecessary
    //public static void RenderFull(EditorViewModel viewModel, IEditorRenderer renderer)
    //{
    //    renderer.BeginFrame(viewModel);

    //    var selection = viewModel.SelectionViewRange?.Normalize();
    //    for (int i = 0; i < viewModel.VisibleLines.Count; i++)
    //    {
    //        var viewLine = viewModel.VisibleLines[i];
    //        int docLine = viewLine.DocumentLineIndex;

    //        SelectionSegment? seg = null;

    //        if (selection is not null && !selection.Value.IsEmpty && selection.Value.ContainsLine(docLine)) 
    //        {
    //            int lineLength = viewLine.Text?.Length ?? 0;
    //            seg = selection.Value.GetSegmentForLine(docLine, lineLength);
    //        }
            
    //        renderer.RenderLine(i, viewLine, seg);
    //    }

    //    renderer.RenderCaret(viewModel.CaretViewPosition);

    //    renderer.EndFrame();
    //}
}
