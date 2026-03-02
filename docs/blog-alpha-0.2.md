# Notepad.Tui Alpha 0.2: Full-Featured TUI Text Editor in .NET 10

After months of steady development, I'm excited to share the Alpha 0.2 release of **notepad.tui** — a terminal-based reimplementation of the classic Notepad app built entirely in .NET 10. This release marks a major milestone: the editor now covers every core editing capability you'd expect from a daily-use text editor, all running in a terminal window via [Terminal.Gui](https://github.com/gui-cs/Terminal.Gui).

---

## What is Notepad.Tui?

Notepad.Tui is a multi-platform text editor project targeting TUI (Terminal User Interface), all sharing a common Core and ViewModel layer. The Alpha 0.2 release focuses on the TUI frontend, built with Terminal.Gui, running on .NET 10.

The goal is fast, no-frills text editing in any context: over SSH, inside a container, at a CI terminal, or anywhere a GUI isn't available.

---

## Architecture: Layered and Testable

One of the design priorities from day one has been **clean layered architecture** with high unit test coverage. The solution is structured into three main layers:

```
D20Tek.Notepad.Core        ← Domain: document model, editing operations, undo/redo
D20Tek.Notepad.ViewModel   ← Presentation logic: viewport, caret, selection, search
D20Tek.Notepad.Tui         ← Terminal.Gui UI: rendering, key/mouse bindings, menus
```

Each layer has its own test project. Alpha 0.2 ships with **944 unit tests** across Core and ViewModel, with extensive coverage on the business logic layers.

## Alpha 0.2 Feature Set

### Text Editing

The fundamentals features have been added: typing, backspace, delete, enter, and tab (configurable as spaces or a tab character). Cut, copy, and paste integrate with the system clipboard via Terminal.Gui's clipboard abstraction, with a platform-specific implementation for each OS.

### Undo and Redo (`Ctrl+Z`, `Ctrl+Y`)

Undo/redo is implemented using a classic command pattern with one important enhancement: **typing groups**. Rapid keystrokes are accumulated into a single `CompositeOperation` using a 2-second idle timer, so that pressing Ctrl+Z after typing a sentence undoes the whole sentence rather than one character at a time - matching the behavior of desktop editors.

### File Operations

- **New** (`Ctrl+N`): prompts to save if dirty, then opens a blank document
- **Open** (`Ctrl+O`): file picker dialog, prompts to save if dirty - *not shown in this release's feature table because it was in Alpha 0.1*
- **Save** (`Ctrl+S`): saves to current path; falls through to Save As if unsaved
- **Save As** (`Ctrl+Shift+S`): file picker dialog

Dirty document tracking uses a version counter on the `UndoStack`. The document is considered clean when the stack version matches the last-saved version, so undo back to a saved state correctly clears the dirty flag.

### Selection and Clipboard

Text selection uses an anchor/caret model. The anchor is set when selection begins and is preserved while extending. `Select All` (`Ctrl+A`) sets anchor to document start and caret to document end.

Mouse selection is fully supported:
- **Click**: moves caret, clears selection
- **Click + drag**: extends selection
- **Double-click**: selects the word under the cursor (word = letters, digits, `_`)
- **Triple-click**: selects the entire document line (works correctly with word wrap)

### Word Wrap

Word wrap is a **view-only** feature — the document model always stores plain lines. When enabled, `WordWrapCalculator` breaks each document line into visual segments at the viewport width, and all rendering, caret positioning, and mouse hit testing operate on the visual segment coordinates, converting back to document coordinates as needed.

### Find and Replace

The `SearchService` in the Core layer implements forward and backward text search across the document's lines, supporting:

- **Case-sensitive** and **case-insensitive** matching
- **Wrap-around** (continues from document start/end when reaching the boundary)

The ViewModel exposes `FindNext`, `FindPrevious`, `FindNextFromCurrent`, `FindPreviousFromCurrent`, `Replace`, and `ReplaceAll`. Replace All wraps all its individual replacements in a `BeginGroup`/`EndGroup` block, so the entire operation is a single undo step.

The TUI layer provides three Terminal.Gui dialogs:

- **Find dialog** (`Ctrl+F`): search term, case-sensitive checkbox, wrap-around checkbox
- **Replace dialog** (`Ctrl+H`): search and replacement term, same options, plus Replace / Replace All buttons
- **Go to Line dialog** (`Ctrl+G`): validates input against the document line count

When opening the Find or Replace dialog, the search field is pre-populated with the currently selected text (if any), falling back to the last search term.

Find Next (`F3`) and Find Previous (`Shift+F3`) are also available as direct key bindings, enabled only after an initial search.

---

## What's Coming Next

With the core editing feature set complete, the focus for the next milestone is:

- **Status bar**: Show current `Ln X, Col Y`, document encoding, and line ending style
- **Enhanced navigation**: Ctrl+Arrow for word navigation, Ctrl+Backspace/Delete for word deletion, Ctrl+Shift+Arrow for word selection
- **Line numbers**: Optional gutter with document line numbers
- **Large file support**: Memory-mapped file loading for files that don't fit in a string
- **Change encoding**: Allow changing the document encoding via a menu option
- **Recent files**: File > Recent submenu with MRU list
- **Zoom**: Ctrl+Plus/Minus to increase/decrease font size (TUI limitations may apply)
 
---

## Try It

Clone the repo and run the TUI app directly:

```bash
git clone https://github.com/d20Tek/notepad.net
cd notepad.net
dotnet run --project src/D20Tek.Notepad.Tui
```

Or open a specific file:

```bash
dotnet run --project src/D20Tek.Notepad.Tui -- myfile.txt
```

Feedback, issues, and contributions are welcome on GitHub.
