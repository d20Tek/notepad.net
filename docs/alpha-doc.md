# Notepad.Tui — Alpha Release (v0.1.0)

A fast, clean, and modern **terminal-based text viewer** for .NET, inspired by classic tools like Notepad and contemporary editor engines.  
This alpha focuses on delivering a rock‑solid **read‑only** experience with smooth navigation, precise selection, and a responsive UI built on top of Terminal.Gui.

This is the first public preview of the project. It’s intentionally limited in scope, but the foundation is strong and ready for community feedback.

---

## Features in This Alpha

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

---

## What’s *Not* Included Yet (By Design)

This alpha is **read‑only**. The following features are planned for future phases:

- Editing (insert, delete, replace)
- Undo/redo
- Saving files
- Word wrap
- Search / find
- Status bar and line/column indicators

The goal of this release is to validate the **interaction model**, **viewport engine**, and **rendering pipeline** before layering on editing semantics.

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