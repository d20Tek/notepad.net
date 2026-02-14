using D20Tek.Notepad.Core.Primitives;
using System.Text;

namespace D20Tek.Notepad.Core.Document;

public sealed record DocumentData(List<TextLine> Lines, Encoding Encoding, LineEndingStyle LineEndingStyle);
