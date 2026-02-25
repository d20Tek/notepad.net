# Editing Feature Implementation

## Overview
Implement text editing functionality at the EditorViewModel and UI layers to enable basic text editing capabilities similar to Windows Notepad:
- Text input (typing characters)
- Backspace and Delete operations
- Enter key for new lines
- Tab key handling
- Undo/Redo support
- Select All
- File operations (New, Save, Save As)

---

## Current State Analysis
The `EditorCommandService` in the Core layer already provides:
- `TypeCharacter(char c)` - Insert a character
- `InsertText(string text)` - Insert text
- `InsertNewLine()` - Insert new line
- `Backspace()` / `Delete()` - Character deletion
- `SelectAll()` - Select all text
- `Undo()` / `Redo()` - Undo/Redo
- `BeginTypingGroup()` / `EndTypingGroup()` - Undo grouping

The `EditorViewModel` currently only exposes Navigation and Selection methods.
The `KeyBindings` only handles navigation keys (arrows, Home/End, Page Up/Down).

---

## Task Breakdown

### Phase 1: EditorViewModel Editing Methods

#### Task 1.1: Add Text Input Methods to EditorViewModel
- [x] Create `EditorViewModel.Editing.cs` partial class
- [x] Add `TypeCharacter(char c)` method that delegates to `Commands.TypeCharacter(c)` and refreshes
- [x] Add `InsertText(string text)` method with refresh
- [x] Add `InsertNewLine()` method with refresh
- [x] Add `InsertTab()` method that inserts tab or spaces based on settings

#### Task 1.2: Add Deletion Methods to EditorViewModel
- [x] Add `Backspace()` method with refresh and caret visibility
- [x] Add `Delete()` method with refresh
- [x] Add `DeleteSelection()` method with refresh

#### Task 1.3: Add Undo/Redo Methods to EditorViewModel
- [x] Add `Undo()` method with refresh
- [x] Add `Redo()` method with refresh
- [x] Add `CanUndo` property
- [x] Add `CanRedo` property
- [x] Add undo grouping for continuous typing

#### Task 1.4: Add Select All to EditorViewModel
- [x] Add `SelectAll()` method with refresh

---

### Phase 2: Key Bindings for Editing

#### Task 2.1: Add Character Input Handling
- [x] Update `EditorView.ProcessKey()` to handle printable characters
- [x] Route character input to `viewModel.TypeCharacter()`
- [x] Handle special characters correctly (spaces, punctuation)

#### Task 2.2: Add Editing Key Bindings
- [x] Add `Backspace` key binding → `vm.Backspace()`
- [x] Add `Delete` key binding → `vm.Delete()`
- [x] Add `Enter` key binding → `vm.InsertNewLine()`
- [x] Add `Tab` key binding → `vm.InsertTab()`
- [ ] Add `Shift+Tab` for potential dedent (optional)

#### Task 2.3: Add Undo/Redo Key Bindings
- [x] Add `Ctrl+Z` key binding → `vm.Undo()`
- [x] Add `Ctrl+Y` key binding → `vm.Redo()`
- [x] Add `Ctrl+Shift+Z` as alternative redo (optional)

#### Task 2.4: Add Select All Key Binding
- [x] Add `Ctrl+A` key binding → `vm.SelectAll()`

---

### Phase 3: File Operations

#### Task 3.1: Add New File Command
- [x] Create `FileNewCommand` class
- [x] Add "New" menu item to File menu (Ctrl+N)
- [x] Prompt to save if document is modified (dirty flag)
- [x] Clear document and reset editor state

#### Task 3.2: Add Save File Command
- [x] Create `FileSaveCommand` class
- [x] Add "Save" menu item to File menu (Ctrl+S)
- [x] Save to current file path if known
- [x] Fall back to "Save As" if no file path
- [x] Update dirty flag after save

#### Task 3.3: Add Save As File Command
- [x] Create `FileSaveAsCommand` class
- [x] Add "Save As..." menu item to File menu (Ctrl+Shift+S)
- [x] Show save file dialog
- [x] Save document to selected path
- [x] Update current file path and dirty flag

#### Task 3.4: Track Document Modified State
- [x] Add `IsDirty` property to EditorViewModel
- [x] Track modifications (typing, paste, delete, etc.)
- [x] Reset dirty flag on save
- [x] Show modified indicator in title bar

#### Task 3.5: Add Unsaved Changes Prompt
- [x] Prompt user when closing with unsaved changes
- [x] Prompt user when opening new file with unsaved changes
- [x] Options: Save, Don't Save, Cancel

---

### Phase 4: Edit Menu

#### Task 4.1: Create Edit Menu Structure
- [x] Add "Edit" menu between "File" and "View"
- [x] Add menu items: Undo, Redo, separator, Select All

#### Task 4.2: Wire Edit Menu Commands
- [x] Create `UndoCommand`, `RedoCommand` 
- [x] Create `SelectAllCommand`
- [x] Register all commands in `CommandRegistry`
- [x] Enable/disable Undo/Redo based on availability

---

### Phase 5: Unit Tests

#### Task 5.1: EditorViewModel Editing Tests
- [x] Test `TypeCharacter()` inserts character at caret
- [x] Test `InsertText()` with selection replaces selection
- [x] Test `InsertNewLine()` splits line
- [x] Test `InsertTab()` inserts correct whitespace
- [x] Test `Backspace()` deletes character before caret
- [x] Test `Delete()` deletes character after caret
- [x] Test `DeleteSelection()` removes selected text

#### Task 5.2: Undo/Redo Tests
- [x] Test `Undo()` reverts last change
- [x] Test `Redo()` re-applies undone change
- [x] Test `CanUndo` returns correct value
- [x] Test `CanRedo` returns correct value
- [x] Test undo grouping for continuous typing

#### Task 5.3: Select All Tests
- [x] Test `SelectAll()` selects entire document
- [x] Test anchor and caret positions after SelectAll

---

## Acceptance Criteria

### Phase 1-2 (Core Editing)
- [x] User can type characters and they appear in the editor
- [x] Backspace deletes character before caret
- [x] Delete key deletes character after caret
- [x] Enter creates a new line
- [x] Tab inserts tab character or spaces
- [x] Ctrl+Z undoes last action
- [x] Ctrl+Y redoes last undone action
- [x] Ctrl+A selects all text

### Phase 3-4 (File Operations & Menu)
- [x] File > New creates empty document (with save prompt)
- [x] File > Save saves current document
- [x] File > Save As shows save dialog
- [x] Edit menu contains Undo, Redo, Select All commands
- [x] Modified documents show dirty indicator
- [x] User is prompted before losing unsaved changes

---

## Dependencies
- `EditorCommandService` (existing in Core)
- `EditorSession` (existing in Core)
- `KeyBindings` (existing in Tui - needs extension)
- `MenuBuilder` (existing in Tui - needs extension)
- Terminal.Gui dialog API

---

## Notes
- File operations should handle encoding properly (UTF-8 with/without BOM)
- Consider showing line/column position in status bar (future enhancement)
