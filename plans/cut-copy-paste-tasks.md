# Cut, Copy, Paste Feature Implementation

## Overview
Implement clipboard operations (Cut, Copy, Paste) for the Notepad.Tui editor with:
- Menu items in the Edit menu with standard keyboard shortcuts
- Dynamic enable/disable based on selection state and clipboard contents
- Integration with the system clipboard via Terminal.Gui's `Clipboard` class

---

## Current State Analysis

### Existing Core Support
The `EditorCommandService` already provides clipboard-like operations:
- `CopySelection()` - Returns selected text without modifying document
- `CutSelection()` - Returns selected text and deletes it from document
- `Paste(string text)` - Inserts text at caret position

The `EditorSession` provides:
- `HasSelection` property - Returns true if anchor != caret
- `GetSelectedText()` - Returns the selected text

### What Needs Implementation
1. **ViewModel layer**: Add Cut, Copy, Paste methods that integrate with system clipboard
2. **UI Commands**: Create `CutCommand`, `CopyCommand`, `PasteCommand` in Tui layer
3. **Menu Items**: Add to Edit menu with CanExecute callbacks
4. **Key Bindings**: Standard Ctrl+X, Ctrl+C, Ctrl+V shortcuts

---

## Task Breakdown

### Phase 1: EditorViewModel Clipboard Methods

#### Task 1.1: Add Clipboard Interface Abstraction
- [x] Create `IClipboardService` interface in ViewModel project
  - `void SetText(string text)`
  - `string? GetText()`
  - `bool ContainsText { get; }`
- [x] Add constructor parameter/property to `EditorViewModel` for clipboard service

#### Task 1.2: Add Cut Method to EditorViewModel
- [x] Add `Cut()` method to `EditorViewModel.Editing.cs`
  - Check if selection exists
  - Call `Commands.CutSelection()` to get text and delete
  - Set text to clipboard via `IClipboardService`
  - Call `CheckDirtyStateChanged()` and `Refresh()`

#### Task 1.3: Add Copy Method to EditorViewModel
- [x] Add `Copy()` method to `EditorViewModel.Editing.cs`
  - Check if selection exists
  - Call `Commands.CopySelection()` to get text
  - Set text to clipboard via `IClipboardService`
  - No refresh needed (document unchanged)

#### Task 1.4: Add Paste Method to EditorViewModel
- [x] Add `Paste()` method to `EditorViewModel.Editing.cs`
  - Get text from clipboard via `IClipboardService`
  - If clipboard has text, call `Commands.Paste(text)`
  - Call `CheckDirtyStateChanged()` and `Refresh()`

#### Task 1.5: Add CanExecute Properties to EditorViewModel
- [x] Add `CanCut` property ? returns `Session.HasSelection`
- [x] Add `CanCopy` property ? returns `Session.HasSelection`
- [x] Add `CanPaste` property ? returns `_clipboardService.ContainsText`

---

### Phase 2: Clipboard Service Implementation

#### Task 2.1: Create TerminalGuiClipboardService
- [ ] Create `TerminalGuiClipboardService` class in Tui project implementing `IClipboardService`
  - `SetText()` ? calls `Terminal.Gui.Clipboard.TrySetClipboardData(text)`
  - `GetText()` ? calls `Terminal.Gui.Clipboard.TryGetClipboardData()` and returns result
  - `ContainsText` ? calls `Terminal.Gui.Clipboard.TryGetClipboardData()` and checks for non-empty

#### Task 2.2: Wire Up Clipboard Service
- [ ] Update `EditorViewFactory` to create and inject `TerminalGuiClipboardService`
- [ ] Update `EditorViewModel` constructor to accept optional `IClipboardService`

---

### Phase 3: UI Commands

#### Task 3.1: Create CutCommand
- [ ] Create `CutCommand` static class in `Commands` folder
  - Define `CommandName = "Cut"`
  - `Execute()` calls `viewModel.Cut()`
  - `Create()` returns `UiCommand` with `Key.CtrlMask | Key.X`

#### Task 3.2: Create CopyCommand
- [ ] Create `CopyCommand` static class in `Commands` folder
  - Define `CommandName = "Copy"`
  - `Execute()` calls `viewModel.Copy()`
  - `Create()` returns `UiCommand` with `Key.CtrlMask | Key.C`

#### Task 3.3: Create PasteCommand
- [ ] Create `PasteCommand` static class in `Commands` folder
  - Define `CommandName = "Paste"`
  - `Execute()` calls `viewModel.Paste()`
  - `Create()` returns `UiCommand` with `Key.CtrlMask | Key.V`

---

### Phase 4: Menu Integration

#### Task 4.1: Register Commands in MenuBuilder
- [ ] Add `commands.Register(CutCommand.Create(viewModel))`
- [ ] Add `commands.Register(CopyCommand.Create(viewModel))`
- [ ] Add `commands.Register(PasteCommand.Create(viewModel))`

#### Task 4.2: Add Menu Items to Edit Menu
- [ ] Add Cut menu item after Redo separator:
  - `new MenuItemDefinition("Cu_t", CutCommand.CommandName, canExecute: () => viewModel.CanCut)`
- [ ] Add Copy menu item:
  - `new MenuItemDefinition("_Copy", CopyCommand.CommandName, canExecute: () => viewModel.CanCopy)`
- [ ] Add Paste menu item:
  - `new MenuItemDefinition("_Paste", PasteCommand.CommandName, canExecute: () => viewModel.CanPaste)`
- [ ] Add separator after Paste, before Select All

---

### Phase 5: Unit Tests

#### Task 5.1: ViewModel Unit Tests
- [x] Create mock `IClipboardService` for testing
- [x] Add `EditorViewModelTests.Clipboard.cs` with tests:
  - `Cut_WithSelection_CutsTextToClipboard`
  - `Cut_WithoutSelection_DoesNothing`
  - `Copy_WithSelection_CopiesToClipboard`
  - `Copy_WithoutSelection_DoesNothing`
  - `Paste_WithClipboardContent_InsertsText`
  - `Paste_WithEmptyClipboard_DoesNothing`
  - `Paste_ReplacesSelection_WhenSelectionActive`
  - `CanCut_ReturnsTrue_WhenSelectionExists`
  - `CanCut_ReturnsFalse_WhenNoSelection`
  - `CanCopy_ReturnsTrue_WhenSelectionExists`
  - `CanCopy_ReturnsFalse_WhenNoSelection`
  - `CanPaste_ReturnsTrue_WhenClipboardHasText`
  - `CanPaste_ReturnsFalse_WhenClipboardEmpty`

#### Task 5.2: Core Layer Tests (Already Exist)
- [x] `CopySelection_ReturnsSelectedText` - already tested
- [x] `CutSelection_ReturnsSelectedTextAndDeletesIt` - already tested
- [x] `CutSelection_WithNoSelection_ReturnsEmptyString` - already tested
- [ ] Verify `Paste` tests exist, add if missing

---

## File Changes Summary

### New Files
| File | Purpose |
|------|---------|
| `src\D20Tek.Notepad.ViewModel\IClipboardService.cs` | Clipboard abstraction interface |
| `src\D20Tek.Notepad.Tui\TerminalGuiClipboardService.cs` | Terminal.Gui clipboard implementation |
| `src\D20Tek.Notepad.Tui\Commands\ClipboardCommands.cs` | Cut, Copy, Paste command definitions |
| `tests\D20Tek.Notepad.ViewModel.UnitTests\EditorViewModelTests.Clipboard.cs` | Clipboard tests |
| `tests\D20Tek.Notepad.ViewModel.UnitTests\MockClipboardService.cs` | Test mock |

### Modified Files
| File | Changes |
|------|---------|
| `src\D20Tek.Notepad.ViewModel\EditorViewModel.cs` | Add IClipboardService field and constructor parameter |
| `src\D20Tek.Notepad.ViewModel\EditorViewModel.Editing.cs` | Add Cut, Copy, Paste methods and CanExecute properties |
| `src\D20Tek.Notepad.Tui\Menus\MenuBuilder.cs` | Register clipboard commands and add menu items |
| `src\D20Tek.Notepad.Tui\EditorViewFactory.cs` | Create and inject clipboard service |

---

## Edit Menu Layout (After Implementation)

```
Edit
??? Undo            Ctrl+Z     [disabled when CanUndo=false]
??? Redo            Ctrl+Y     [disabled when CanRedo=false]
??? ?????????????????
??? Cut             Ctrl+X     [disabled when no selection]
??? Copy            Ctrl+C     [disabled when no selection]
??? Paste           Ctrl+V     [disabled when clipboard empty]
??? ?????????????????
??? Select All      Ctrl+A
```

---

## Technical Notes

### Terminal.Gui Clipboard API
```csharp
// Set clipboard text
Clipboard.TrySetClipboardData("text to copy");

// Get clipboard text
if (Clipboard.TryGetClipboardData(out string? data) && !string.IsNullOrEmpty(data))
{
    // Use data
}
```

### CanExecute Evaluation
The `MenuItemDefinition.CanExecute` callback is evaluated when the menu is opened, ensuring Cut/Copy are disabled when there's no selection and Paste is disabled when the clipboard is empty.

---

## Acceptance Criteria

1. **Cut (Ctrl+X)**
   - [ ] When selection exists: copies selected text to clipboard and removes from document
   - [ ] When no selection: does nothing
   - [ ] Menu item disabled when no selection

2. **Copy (Ctrl+C)**
   - [ ] When selection exists: copies selected text to clipboard
   - [ ] When no selection: does nothing
   - [ ] Menu item disabled when no selection
   - [ ] Document remains unchanged

3. **Paste (Ctrl+V)**
   - [ ] When clipboard has text: inserts at caret position
   - [ ] When selection exists: replaces selection with clipboard content
   - [ ] When clipboard empty: does nothing
   - [ ] Menu item disabled when clipboard empty

4. **Undo/Redo**
   - [ ] Cut operation can be undone (text restored)
   - [ ] Paste operation can be undone

5. **Dirty State**
   - [ ] Cut marks document as dirty
   - [ ] Copy does not affect dirty state
   - [ ] Paste marks document as dirty
