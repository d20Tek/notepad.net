# Notepad.Tui — Alpha Release (v0.2.0)

A fast, clean, and modern **terminal-based text viewer** for .NET, inspired by classic tools like Notepad and contemporary editor engines.  
This alpha focuses on delivering a rock‑solid view and editing experience with smooth navigation, precise selection, and a responsive UI built on top of Terminal.Gui.

This is the second public preview of the project. It’s intentionally limited in scope, but the foundation is strong and ready for community feedback. This second release includes editing capabilities, undo/redo, and cut/copy/paste support, and find/replace dialogs.

This update introduced the editing features, but is still in the early stages of development. The core architecture is in place, but there are still new features planned in future releases. Please try out the tool and share any feedback or issues you encounter!

---

## Features in the Alpha

### ✔️ Smooth, predictable navigation
- Arrow keys, PageUp/PageDown, Home/End  
- Ctrl+Home / Ctrl+End  
- Horizontal navigation with automatic viewport scrolling  
- Vertical navigation with correct caret visibility

### ✔️ Mouse support
- Click to move caret  
- Click‑and‑drag selection  
- Horizontal drag‑scrolling  
- Vertical drag‑scrolling (auto‑scroll when dragging above/below viewport)  
- Scroll wheel support

### ✔️ Selection behavior that feels like a real editor
- Multi‑line selection  
- Zero‑length (caret‑only) selection  
- Correct clipping when selection is partially visible  
- No “sticky” selection artifacts when scrolling  
- Accurate mapping between document and viewport coordinates

### ✔️ Rendering engine
- Efficient line rendering  
- Horizontal clipping  
- Selection segment rendering  
- Dark theme included  
- Clean separation between document model, view model, and renderer

### ✔️ Resize‑aware UI
- Terminal and viewport resizing handled gracefully  
- Caret and selection remain consistent after resize  
- No redraw artifacts or flicker

### ✔️ Word wrap
- Menu toggle to turn word wrap on/off  
- Wrap long text lines within the view width

### ✔️ Text editing capabilities
- Write text, insert, delete, and replace text  
- Undo/redo support for text changes
- Save and Save As functionality to write changes back to disk

### ✔️ Find and Replace
- Ability to search for text within the document (find next/previous)
- Replace functionality to substitute text with new content
- Support for case-sensitive and whole-word search options
- User-friendly dialogs for find and replace operations
- Highlighting of search results within the document
- Go to line functionality to quickly navigate to specific lines in the document

---

## What’s *Not* Included Yet (By Design)

We don't provide the full set of features yet. The following features are planned for future phases:

- Status bar with line/column indicators
- Line number column in the editor view
- Further navigation enchancements
- Change file encoding
- Triple-click selection for lines
- Large file support using memory-mapped files
- Recent files submenu
- Zoom? (TUI limitations may apply)

The goal of this release is to validate the **interaction model**, **viewport engine**, and **editing capabilities**.

---

## Architecture Overview

Notepad.Tui is built on a layered design:

- **Document Model**  
  Encoding‑aware, line‑ending‑aware, deterministic text model.

- **View Model**  
  Tracks viewport offsets, caret position, selection, and visible lines.

- **Renderer**  
  Draws only what’s visible, with correct clipping and selection segments.

- **Terminal.Gui Client**  
  Handles input, mouse events, resizing, and dispatches to the view model.

This architecture is designed for long‑term maintainability and future features like editing, incremental rendering, and memory-mapped huge files.

---

## Installing & Running

```bash
dotnet run -- path/to/file.txt
```