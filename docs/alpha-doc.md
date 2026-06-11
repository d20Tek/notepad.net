# Notepad.Tui — Alpha Release (v0.3.2)

A fast, clean, and modern **terminal-based text viewer** for .NET, inspired by classic tools like Notepad and contemporary editor engines.  
This alpha focuses on delivering a rock‑solid view and editing experience with smooth navigation, precise selection, and a responsive UI built on top of Terminal.Gui.

This is the fourth public preview of the project. This release includes a comprehensive set of features that establish the core editing and navigation capabilities, including loading and editing large files using a memory mapped file.

---

## Cumulative Features

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

## Features Added this Release

### ✔️ Enhanced Keyboard Navigation
- Move to next/previous word with Ctrl+Arrow keys  
- Select next/previous word with Ctrl+Shift+Arrow keys
- Delete next/previous word with Ctrl+Backspace and Ctrl+Delete
- Triple-click to select entire line (works with word wrap)

### ✔️ Status Bar
- Implemented StatusBar view to show line/column, encoding, and line ending information
- Updates dynamically as the caret moves and document changes
- View > Status bar menu option to toggle visibility of the status bar

### ✔️ Change File Encoding
- Added support for changing the file encoding through a menu options
- Encoding submenu with options for UTF-8, UTF-16 LE/BE, and UTF-8 with BOM
- Undo/redo operation to change encoding and update the document model accordingly
- Ensure document IsDirty flag is set when encoding changes

### ✔️ Additional Editing Features
- Line number gutter in the editor view (optional toggle via View menu)
- Recent files submenu to quickly access recently opened documents (last 10)
- Zoom behavior with status bar hint to use terminal zoom, and message box if status bar is hidden
- Insert/Overwrite mode toggle
- Help/About dialog with version info & link to documentation
- Duplicate line or selection.
- Move line up and down.
- Indent and outdent selection or line.
- Trim trailing whitespace from the current line.

### ✔️ Large File Support with Memory-Mapped Files
- Loads files of 10MB or larger using memory-mapped files for efficient access.
- Fixed performance problems with multi-pass calculations that caused slow loading of large files.
- Fixed performance issues in typing and navigation in large files by removing O(N) operations.

---

## What’s *Not* Included Yet (By Design)

The following features are planned for future updates:

| Feature | Shortcut | Rationale |
|---------|----------|-----------|
| Status Bar Enhancements | | word count and selection length. StatusDetails just needs two new fields. |

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

The Notepad.Tui tool is available as a global .NET tool installable from NuGet.org. You can install it using the following command:

```bash
dotnet tool install --global notepad.tui --version 0.3.4
```