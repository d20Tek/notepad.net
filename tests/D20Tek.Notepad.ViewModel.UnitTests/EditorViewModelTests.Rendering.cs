using D20Tek.Notepad.Core.Document;
using D20Tek.Notepad.Core.Editing;
using D20Tek.Notepad.Core.Primitives;
using D20Tek.Notepad.ViewModel.Rendering;

namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelRenderingTests
{
    private static readonly DocumentFactory _docFactory = new();

    // RenderFrame tests
    [TestMethod]
    public void RenderFrame_CallsBeginFrameFirst()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello", "World"]);
        viewModel.SetViewportHeight(2);
        var renderer = new TestRenderer();

        // act
        viewModel.RenderFrame(renderer);

        // assert
        Assert.IsTrue(renderer.BeginFrameCalled);
        Assert.AreEqual(0, renderer.CallOrder.IndexOf("BeginFrame"));
    }

    [TestMethod]
    public void RenderFrame_CallsEndFrameLast()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello", "World"]);
        viewModel.SetViewportHeight(2);
        var renderer = new TestRenderer();

        // act
        viewModel.RenderFrame(renderer);

        // assert
        Assert.IsTrue(renderer.EndFrameCalled);
        Assert.AreEqual(renderer.CallOrder.Count - 1, renderer.CallOrder.IndexOf("EndFrame"));
    }

    [TestMethod]
    public void RenderFrame_RendersAllVisibleLines()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Line 1", "Line 2", "Line 3"]);
        viewModel.SetViewportHeight(3);
        var renderer = new TestRenderer();

        // act
        viewModel.RenderFrame(renderer);

        // assert
        Assert.AreEqual(3, renderer.RenderedLines.Count);
        Assert.AreEqual("Line 1", renderer.RenderedLines[0].Line.Text);
        Assert.AreEqual("Line 2", renderer.RenderedLines[1].Line.Text);
        Assert.AreEqual("Line 3", renderer.RenderedLines[2].Line.Text);
    }

    [TestMethod]
    public void RenderFrame_RendersLinesWithCorrectViewLineIndex()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Line 1", "Line 2", "Line 3"]);
        viewModel.SetViewportHeight(3);
        var renderer = new TestRenderer();

        // act
        viewModel.RenderFrame(renderer);

        // assert
        Assert.AreEqual(0, renderer.RenderedLines[0].ViewLineIndex);
        Assert.AreEqual(1, renderer.RenderedLines[1].ViewLineIndex);
        Assert.AreEqual(2, renderer.RenderedLines[2].ViewLineIndex);
    }

    [TestMethod]
    public void RenderFrame_RendersCaret()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);
        viewModel.SetViewportHeight(1);
        viewModel.Refresh();
        var renderer = new TestRenderer();

        // act
        viewModel.RenderFrame(renderer);

        // assert
        Assert.IsTrue(renderer.CaretRendered);
        Assert.AreEqual(new ViewPosition(0, 5), renderer.RenderedCaretPosition);
    }

    [TestMethod]
    public void RenderFrame_WithSelection_RendersLinesWithSelectionSegments()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Anchor = new TextPosition(0, 2);
        session.Caret = new TextPosition(0, 7);
        viewModel.SetViewportHeight(1);
        viewModel.Refresh();
        var renderer = new TestRenderer();

        // act
        viewModel.RenderFrame(renderer);

        // assert
        Assert.AreEqual(1, renderer.RenderedLines.Count);
        Assert.IsNotNull(renderer.RenderedLines[0].Selection);
        Assert.AreEqual(2, renderer.RenderedLines[0].Selection!.Value.StartColumn);
        Assert.AreEqual(7, renderer.RenderedLines[0].Selection!.Value.EndColumn);
    }

    [TestMethod]
    public void RenderFrame_WithNoSelection_RendersLinesWithNullSelectionSegment()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);
        viewModel.SetViewportHeight(1);
        viewModel.Refresh();
        var renderer = new TestRenderer();

        // act
        viewModel.RenderFrame(renderer);

        // assert
        Assert.AreEqual(1, renderer.RenderedLines.Count);
        Assert.IsNull(renderer.RenderedLines[0].Selection);
    }

    [TestMethod]
    public void RenderFrame_WithMultiLineSelection_RendersCorrectSegmentsPerLine()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World", "Second Line", "Third Line"]);
        session.Anchor = new TextPosition(0, 6);
        session.Caret = new TextPosition(2, 5);
        viewModel.SetViewportHeight(3);
        viewModel.Refresh();
        var renderer = new TestRenderer();

        // act
        viewModel.RenderFrame(renderer);

        // assert
        Assert.AreEqual(3, renderer.RenderedLines.Count);
        
        // First line: selection from column 6 to end (11)
        Assert.IsNotNull(renderer.RenderedLines[0].Selection);
        Assert.AreEqual(6, renderer.RenderedLines[0].Selection!.Value.StartColumn);
        
        // Middle line: full selection
        Assert.IsNotNull(renderer.RenderedLines[1].Selection);
        Assert.AreEqual(0, renderer.RenderedLines[1].Selection!.Value.StartColumn);
        
        // Last line: selection from 0 to column 5
        Assert.IsNotNull(renderer.RenderedLines[2].Selection);
        Assert.AreEqual(0, renderer.RenderedLines[2].Selection!.Value.StartColumn);
        Assert.AreEqual(5, renderer.RenderedLines[2].Selection!.Value.EndColumn);
    }

    [TestMethod]
    public void RenderFrame_CallsMethodsInCorrectOrder()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Line 1", "Line 2"]);
        viewModel.SetViewportHeight(2);
        var renderer = new TestRenderer();

        // act
        viewModel.RenderFrame(renderer);

        // assert
        var expectedOrder = new[] { "BeginFrame", "RenderLine", "RenderLine", "RenderCaret", "EndFrame" };
        CollectionAssert.AreEqual(expectedOrder, renderer.CallOrder);
    }

    [TestMethod]
    public void RenderFrame_WithEmptyViewport_OnlyCallsBeginEndAndCaret()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(0);
        var renderer = new TestRenderer();

        // act
        viewModel.RenderFrame(renderer);

        // assert
        Assert.AreEqual(0, renderer.RenderedLines.Count);
        Assert.IsTrue(renderer.BeginFrameCalled);
        Assert.IsTrue(renderer.EndFrameCalled);
        Assert.IsTrue(renderer.CaretRendered);
    }

    [TestMethod]
    public void RenderFrame_PassesViewModelToBeginFrame()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(1);
        var renderer = new TestRenderer();

        // act
        viewModel.RenderFrame(renderer);

        // assert
        Assert.AreSame(viewModel, renderer.ReceivedViewModel);
    }

    // Helper methods
    private static (EditorViewModel viewModel, EditorSession session) CreateViewModel(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var navigation = new EditorNavigationService();
        var commandService = new EditorCommandService(session, navigation);
        var viewModel = new EditorViewModel(session, commandService);
        return (viewModel, session);
    }

    // Test renderer implementation
    [ExcludeFromCodeCoverage]
    private class TestRenderer : IEditorRenderer
    {
        public bool BeginFrameCalled { get; private set; }
        public bool EndFrameCalled { get; private set; }
        public bool CaretRendered { get; private set; }
        public ViewPosition RenderedCaretPosition { get; private set; }
        public EditorViewModel? ReceivedViewModel { get; private set; }
        public List<RenderedLineInfo> RenderedLines { get; } = [];
        public List<string> CallOrder { get; } = [];

        public void BeginFrame(EditorViewModel viewModel)
        {
            BeginFrameCalled = true;
            ReceivedViewModel = viewModel;
            CallOrder.Add("BeginFrame");
        }

        public void RenderLine(int viewLineIndex, ViewLine line, SelectionSegment? selection)
        {
            RenderedLines.Add(new RenderedLineInfo(viewLineIndex, line, selection));
            CallOrder.Add("RenderLine");
        }

        public void RenderCaret(ViewPosition caret)
        {
            CaretRendered = true;
            RenderedCaretPosition = caret;
            CallOrder.Add("RenderCaret");
        }

        public void EndFrame()
        {
            EndFrameCalled = true;
            CallOrder.Add("EndFrame");
        }

        public record RenderedLineInfo(int ViewLineIndex, ViewLine Line, SelectionSegment? Selection);
    }
}