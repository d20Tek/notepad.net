# Insert/Overwrite Mode Feature

## Overview
Add Insert/Overwrite toggle mode to the editor. The default is **insert mode** (existing
behaviour). Pressing the `Insert` key, or selecting `View > Overwrite Mode`, switches to
**overwrite mode** where typed characters replace the character at the caret position instead
of pushing it right. The active mode is reflected by a `OVR`/`INS` indicator in the status
bar and by a block cursor shape in overwrite mode.

**Shortcut**: `Insert` key  
**Menu**: `View > _Overwrite Mode` (checkable, same toggle as Insert key)

---

## Behaviour Specification

| Situation | Insert mode | Overwrite mode |
|---|---|---|
| Type char — mid-line, no selection | Inserts before caret | Replaces char at caret |
| Type char — at line end | Appends char | Appends char (nothing to overwrite) |
| Type char — active selection present | Replaces selection | Replaces selection (same) |
| `Enter` | Split line | Split line (same) |
| `Backspace` / `Delete` | Normal | Normal (same) |
| `Tab` | Insert spaces/tab | Insert spaces/tab (same) |
| Status bar (when visible) | Shows `INS` | Shows `OVR` |
| Caret shape | Line cursor | Block cursor |

---

## Architecture Design

### Data flow — typing in overwrite mode
```
User presses 'X' mid-line (overwrite mode, no selection)
  ? KeyBindings ? viewModel.TypeCharacter('X')
    ? _isOverwriteMode true, no selection, not at line end
      ? Commands.OverwriteCharacter('X')
        ? EditorSession.OverwriteCharacter('X')
          ? ReplaceRangeOperation(caret?caret+1, "X", oldChar, caret)
            ? undo stack records replace (grouped with typing group)
```

### Data flow — toggling the mode
```
User presses Insert key (or selects View > Overwrite Mode)
  ? KeyBindings / OverwriteModeCommand ? viewModel.ToggleInsertMode()
    ? _isOverwriteMode flips
    ? Refresh() ? BuildStatus() includes new IsOverwriteMode
      ? StatusChanged fires ? StatusBarView redraws OVR/INS
    ? InsertModeChanged fires
      ? EditorView.PositionCursor() switches cursor shape (line ? block)
      ? Menu checkmark updates (standard Terminal.Gui checkable item)
```

### `OverwriteCharacter` primitive
A new `EditorSession.OverwriteCharacter(char c)` method that:
- Falls back to `InsertText` when a selection is active or the caret is at line end.
- Otherwise builds a one-character `TextRange` from `Caret` to `Caret + 1` and issues a
  `ReplaceRangeOperation` (already exists). This integrates with the existing undo/redo
  stack and typing-group mechanism with zero changes to those systems.

### `IsOverwriteMode` state
Session-level state — **not persisted** in `EditorSettings`. Lives in a new
`EditorViewModel.InsertMode.cs` partial class (following the `LineNumbers.cs` pattern),
defaulting to `false` (insert mode).

### `IsOverwriteMode` in `StatusDetails`
Add `bool IsOverwriteMode` as the last parameter of the `StatusDetails` record.
`BuildStatus()` reads `IsOverwriteMode` from the ViewModel. `StatusBarView` appends
`INS` or `OVR` to the right section: `"INS | UTF-8 | CRLF"`.

---

## Phase 1: Core — `OverwriteCharacter` Primitive

### Task 1.1: Add `OverwriteCharacter` to `EditorSession.Commands.cs`
File: `src\D20Tek.Notepad.Core\Editing\EditorSession.Commands.cs`
- [x] Add `public void OverwriteCharacter(char c)`:
  - If `HasSelection` ? `ReplaceSelection(c.ToString())` and return
  - If `Caret.Column >= Document.Lines[Caret.Line].Content.Length` ? `InsertText(c.ToString())` and return
  - Otherwise: build `range = new TextRange(Caret, new TextPosition(Caret.Line, Caret.Column + 1))`
  - Capture `oldChar = Document.Lines[Caret.Line].Content[Caret.Column].ToString()`
  - Execute `new ReplaceRangeOperation(range, c.ToString(), oldChar, Caret)`

### Task 1.2: Add `OverwriteCharacter` to `EditorCommandService.cs`
File: `src\D20Tek.Notepad.Core\Editing\EditorCommandService.cs`
- [x] Add `public void OverwriteCharacter(char c) => _session.OverwriteCharacter(c);`
  under the `TypeCharacter` method

### Task 1.3: Unit tests for `EditorSession.OverwriteCharacter`
File: `tests\D20Tek.Notepad.Core.UnitTests\Editing\EditorSessionTests.Commands.cs`
- [x] `OverwriteCharacter_MidLine_ReplacesCharAtCaret`
- [x] `OverwriteCharacter_AtLineEnd_InsertsCharacter`
- [x] `OverwriteCharacter_WithSelection_ReplacesSelection`
- [x] `OverwriteCharacter_MidLine_AdvancesCaretByOne`
- [x] `OverwriteCharacter_IsUndoable`

### Task 1.4: Unit tests for `EditorCommandService.OverwriteCharacter`
File: `tests\D20Tek.Notepad.Core.UnitTests\Editing\EditorCommandServiceTests.Editing.cs`
- [x] `OverwriteCharacter_MidLine_ReplacesCharAtCaret`
- [x] `OverwriteCharacter_AtLineEnd_InsertsCharacter`

---

## Phase 2: ViewModel — Insert Mode State

### Task 2.1: Create `EditorViewModel.InsertMode.cs`
File: `src\D20Tek.Notepad.ViewModel\EditorViewModel.InsertMode.cs`
- [x] Declare `public sealed partial class EditorViewModel`
- [x] Add `private bool _isOverwriteMode = false;`
- [x] Add `public bool IsOverwriteMode => _isOverwriteMode;`
- [x] Add `public event Action<bool>? InsertModeChanged;`
- [x] Add `public void ToggleInsertMode()`:
  - Flip `_isOverwriteMode`
  - Call `Refresh()` (so `BuildStatus()` re-evaluates and fires `StatusChanged`)
  - Fire `InsertModeChanged?.Invoke(_isOverwriteMode)`

### Task 2.2: Modify `TypeCharacter` in `EditorViewModel.Editing.cs`
File: `src\D20Tek.Notepad.ViewModel\EditorViewModel.Editing.cs`
- [x] Inside `TypeCharacter(char c)`, before calling `Commands.TypeCharacter(c)`, branch:
  ```csharp
  bool atLineEnd = Session.Caret.Column >= Session.GetLineLength(Session.Caret.Line);

  if (_isOverwriteMode && !Session.HasSelection && !atLineEnd)
      Commands.OverwriteCharacter(c);
  else
      Commands.TypeCharacter(c);
  ```

### Task 2.3: Unit tests for `EditorViewModel.InsertMode`
File: `tests\D20Tek.Notepad.ViewModel.UnitTests\EditorViewModelTests.InsertMode.cs`
- [x] `IsOverwriteMode_Default_IsFalse`
- [x] `ToggleInsertMode_WhenInsertMode_SetsOverwriteMode`
- [x] `ToggleInsertMode_WhenOverwriteMode_SetsInsertMode`
- [x] `ToggleInsertMode_FiresInsertModeChangedEvent`
- [x] `ToggleInsertMode_FiresStatusChangedEvent`
- [x] `TypeCharacter_WhenInsertMode_InsertsChar`
- [x] `TypeCharacter_WhenOverwriteMode_MidLine_ReplacesChar`
- [x] `TypeCharacter_WhenOverwriteMode_AtLineEnd_AppendsChar`
- [x] `TypeCharacter_WhenOverwriteMode_WithSelection_ReplacesSelection`

---

## Phase 3: Status Bar — `IsOverwriteMode` in `StatusDetails`

### Task 3.1: Add `IsOverwriteMode` to `StatusDetails`
File: `src\D20Tek.Notepad.ViewModel\StatusDetails.cs`
- [x] Add `bool IsOverwriteMode` as the last parameter of the record
- [x] Update `StatusDetails.Empty` to pass `IsOverwriteMode: false`

### Task 3.2: Update `BuildStatus()` in `EditorViewModel.Status.cs`
File: `src\D20Tek.Notepad.ViewModel\EditorViewModel.Status.cs`
- [x] Add `IsOverwriteMode: IsOverwriteMode` to the `new StatusDetails(...)` call in `BuildStatus()`

### Task 3.3: Update `StatusBarView` to render mode indicator
File: `src\D20Tek.Notepad.Tui\StatusBarView.cs`
- [x] Update `FormatEncoding(StatusDetails s)` to prefix the mode indicator:
  `$"{(s.IsOverwriteMode ? "OVR" : "INS")} | {s.DocumentEncoding} | {s.LineEndingStyle}"`

### Task 3.4: Update `StatusDetailsTests`
File: `tests\D20Tek.Notepad.ViewModel.UnitTests\StatusDetailsTests.cs`
- [x] Update `Constructor_WithValidParameters_SetsProperties` — add `IsOverwriteMode: false`
  and assert `Assert.IsFalse(status.IsOverwriteMode)`
- [x] Update `With_ChangingAllProperties_CreatesNewInstanceWithAllUpdates` — flip
  `IsOverwriteMode` to `true` and verify in the assert block
- [x] Update `Equality_SameValues_AreEqual` and `Equality_DifferentValues_AreNotEqual` tests
  to include `IsOverwriteMode` in the constructed instances

---

## Phase 4: UI — Command, Menu, Key Binding, Cursor Shape

### Task 4.1: Create `OverwriteModeCommand.cs`
File: `src\D20Tek.Notepad.Tui\Commands\OverwriteModeCommand.cs`
- [x] Define `internal static class OverwriteModeCommand`
- [x] Add `public const string CommandName = "ToggleOverwriteMode";`
- [x] Add `public static void Execute(EditorViewModel viewModel)`:
  - Guard `viewModel` not null
  - Call `viewModel.ToggleInsertMode()`
- [x] Add `public static UiCommand Create(EditorViewModel viewModel)`:
  - Return `new(CommandName, () => Execute(viewModel), Key.InsertChar)`

### Task 4.2: Register command in `MenuCommands.cs`
File: `src\D20Tek.Notepad.Tui\Menus\MenuCommands.cs`
- [x] Add `commands.Register(OverwriteModeCommand.Create(viewModel));` in the View commands section

### Task 4.3: Add `_Overwrite Mode` to the View menu in `MenuDefinitions.cs`
File: `src\D20Tek.Notepad.Tui\Menus\MenuDefinitions.cs`
- [x] Add checkable `CommandEntry` to the `_View` `TopLevelMenu` after `_Line Numbers`:
  ```csharp
  new CommandEntry(
      "_Overwrite Mode",
      OverwriteModeCommand.CommandName,
      IsCheckable: true,
      IsChecked: () => viewModel.IsOverwriteMode),
  ```

### Task 4.4: Add `Insert` key binding to `KeyBindings.cs`
File: `src\D20Tek.Notepad.Tui\Input\KeyBindings.cs`
- [x] Add to the dictionary in the editing section:
  `[(Key.InsertChar, false, false)] = vm => vm.ToggleInsertMode(),`

### Task 4.5: Switch cursor shape in `EditorView.PositionCursor()`
File: `src\D20Tek.Notepad.Tui\EditorView.cs`
- [x] Use `CursorVisibility.Invisible` in overwrite mode and `CursorVisibility.VerticalFix` in insert mode
- [x] Draw a **software block cursor** in `Redraw` via `DrawOverwriteBlockCursor()` — renders the
  character at the caret with inverted foreground/background colors, works in all terminals
- [x] Subscribe to `_viewModel.InsertModeChanged` in the constructor:
  `_viewModel.InsertModeChanged += OnInsertModeChanged;`
- [x] Add handler: `private void OnInsertModeChanged(bool _) => SetNeedsDisplay();`
- [x] Unsubscribe in `Dispose()`:
  `_viewModel.InsertModeChanged -= OnInsertModeChanged;`

---

## File Changes Summary

### New Files
| File | Purpose |
|---|---|
| `src\D20Tek.Notepad.ViewModel\EditorViewModel.InsertMode.cs` | Partial class: `IsOverwriteMode`, `ToggleInsertMode()`, `InsertModeChanged` event |
| `src\D20Tek.Notepad.Tui\Commands\OverwriteModeCommand.cs` | Toggles insert/overwrite mode; shortcut `Key.InsertChar` |
| `tests\D20Tek.Notepad.ViewModel.UnitTests\EditorViewModelTests.InsertMode.cs` | 9 ViewModel insert-mode tests |

### Modified Files
| File | Change |
|---|---|
| `src\D20Tek.Notepad.Core\Editing\EditorSession.Commands.cs` | Add `OverwriteCharacter(char c)` |
| `src\D20Tek.Notepad.Core\Editing\EditorCommandService.cs` | Delegate `OverwriteCharacter` to session |
| `src\D20Tek.Notepad.ViewModel\EditorViewModel.Editing.cs` | Branch `TypeCharacter` on overwrite mode |
| `src\D20Tek.Notepad.ViewModel\StatusDetails.cs` | Add `IsOverwriteMode` parameter |
| `src\D20Tek.Notepad.ViewModel\EditorViewModel.Status.cs` | Pass `IsOverwriteMode` to `BuildStatus()` |
| `src\D20Tek.Notepad.Tui\StatusBarView.cs` | Render `OVR`/`INS` in right section |
| `src\D20Tek.Notepad.Tui\Menus\MenuCommands.cs` | Register `OverwriteModeCommand` |
| `src\D20Tek.Notepad.Tui\Menus\MenuDefinitions.cs` | Add `_Overwrite Mode` checkable item to View menu |
| `src\D20Tek.Notepad.Tui\Input\KeyBindings.cs` | Add `Key.InsertChar` binding |
| `src\D20Tek.Notepad.Tui\EditorView.cs` | Block cursor in overwrite; subscribe `InsertModeChanged` |
| `tests\D20Tek.Notepad.Core.UnitTests\Editing\EditorSessionTests.Commands.cs` | 5 session overwrite tests |
| `tests\D20Tek.Notepad.Core.UnitTests\Editing\EditorCommandServiceTests.Editing.cs` | 2 command service overwrite tests |
| `tests\D20Tek.Notepad.ViewModel.UnitTests\StatusDetailsTests.cs` | Cover new `IsOverwriteMode` property |

---

## Acceptance Criteria

- [x] Default mode is insert (existing typing behaviour unchanged)
- [x] Pressing `Insert` key toggles between insert and overwrite mode
- [x] `View > Overwrite Mode` menu item toggles the same state; checkmark reflects current mode
- [x] In overwrite mode, typing a char mid-line replaces the char at the caret
- [x] In overwrite mode, typing a char at line end appends (no overwrite beyond EOL)
- [x] In overwrite mode, typing with an active selection replaces the selection
- [x] `Enter`, `Backspace`, `Delete`, and `Tab` behave identically in both modes
- [x] Status bar shows `INS` in insert mode and `OVR` in overwrite mode (when status bar is visible)
- [x] Caret renders as a line cursor (I-beam) in insert mode and a block cursor in overwrite mode
- [x] Overwrite operations are undoable/redoable and group with the existing typing-group mechanism
- [x] Mode resets to insert on every application launch (not persisted to `editor-settings.json`)
