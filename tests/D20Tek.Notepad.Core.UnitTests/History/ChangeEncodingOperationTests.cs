using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.History;

[TestClass]
public class ChangeEncodingOperationTests
{
    [TestMethod]
    public void Redo_WithNewEncoding_SetsDocumentEncoding()
    {
        // arrange
        var document = new Doc.Document([new TextLine("Hello")]);
        var session = new EditorSession(document);
        var operation = new ChangeEncodingOperation(new UTF8Encoding(false), Encoding.Unicode);

        // act
        operation.Redo(session);

        // assert
        Assert.AreEqual(Encoding.Unicode.WebName, document.Encoding.WebName);
    }

    [TestMethod]
    public void Undo_AfterRedo_RestoresPreviousEncoding()
    {
        // arrange
        var previousEncoding = new UTF8Encoding(false);
        var document = new Doc.Document([new TextLine("Hello")]);
        var session = new EditorSession(document);
        var operation = new ChangeEncodingOperation(previousEncoding, Encoding.Unicode);
        operation.Redo(session);

        // act
        operation.Undo(session);

        // assert
        Assert.AreEqual(previousEncoding.WebName, document.Encoding.WebName);
        Assert.IsTrue(document.Encoding.GetPreamble().SequenceEqual(previousEncoding.GetPreamble()));
    }

    [TestMethod]
    public void Redo_Utf8NoBomToUtf8Bom_DistinguishesByPreamble()
    {
        // arrange
        var document = new Doc.Document([new TextLine("Hello")]);
        var session = new EditorSession(document);
        var operation = new ChangeEncodingOperation(new UTF8Encoding(false), new UTF8Encoding(true));

        // act
        operation.Redo(session);

        // assert
        Assert.AreEqual("utf-8", document.Encoding.WebName);
        Assert.IsTrue(document.Encoding.GetPreamble().SequenceEqual(new UTF8Encoding(true).GetPreamble()));
    }

    [TestMethod]
    public void Undo_Utf8BomToPreviousUtf8NoBom_DistinguishesByPreamble()
    {
        // arrange
        var document = new Doc.Document([new TextLine("Hello")]);
        var session = new EditorSession(document);
        var operation = new ChangeEncodingOperation(new UTF8Encoding(false), new UTF8Encoding(true));
        operation.Redo(session);

        // act
        operation.Undo(session);

        // assert
        Assert.AreEqual("utf-8", document.Encoding.WebName);
        Assert.IsTrue(document.Encoding.GetPreamble().SequenceEqual(new UTF8Encoding(false).GetPreamble()));
    }

    [TestMethod]
    public void Redo_MarksDocumentAsModified()
    {
        // arrange
        var document = new Doc.Document([new TextLine("Hello")]);
        var session = new EditorSession(document);
        var operation = new ChangeEncodingOperation(new UTF8Encoding(false), Encoding.Unicode);

        // act
        operation.Redo(session);

        // assert
        Assert.IsTrue(document.IsModified);
    }

    [TestMethod]
    public void Undo_MarksDocumentAsModified()
    {
        // arrange
        var document = new Doc.Document([new TextLine("Hello")]);
        var session = new EditorSession(document);
        var operation = new ChangeEncodingOperation(new UTF8Encoding(false), Encoding.Unicode);
        operation.Redo(session);

        // act
        operation.Undo(session);

        // assert
        Assert.IsTrue(document.IsModified);
    }
}
