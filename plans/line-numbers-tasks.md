# Line Numbers Feature Tasks

## Overview
Implement an optional line number gutter on the left side of the `EditorView`. The gutter
displays 1-based document line numbers, adapts its width to the total line count, uses a
distinct color scheme, and can be toggled via the View menu. In word-wrap mode only the
first visual segment of each document line shows a number; continuation segments show
blank space.

---

## Tasks

### ViewModel Layer

- [x] **LN-01 — Add `LineNumbersEnabled` to `EditorSettings`**
  - Add `bool LineNumbersEnabled { get; init; } = false;` to the `EditorSettings` record.
  - `SettingsService` persists it automatically via JSON.

- [x] **LN-02 — Add line-numbers state and behaviour to `EditorViewModel`**
  - Create `src/D20Tek.Notepad.ViewModel/EditorViewModel.LineNumbers.cs` as a new partial class file.
  - Add `event Action<bool>? LineNumbersChanged;` to `EditorViewModel`.
  - Add `bool IsLineNumbersEnabled => _settings.LineNumbersEnabled;` property.
  - Add `int GutterWidth` computed property: when disabled returns `0`; when enabled returns
    `TotalLines.ToString().Length + 1` (digits in total line count plus one space of padding).
  - Add `ToggleLineNumbers()` method: flips `_settings.LineNumbersEnabled` and fires
    `LineNumbersChanged`. Do **not** call `Refresh()` directly — follow the same pattern
    as `ToggleStatusBar`. The `LineNumbersChanged` event causes `EditorView` to call
    `SetNeedsDisplay()`, which triggers `Redraw()` ? `UpdateViewportSize()` ? 
    `SetViewportWidth()` (with new gutter-adjusted width) ? `SetWithRefresh()` ?
    `Refresh()`. Calling `Refresh()` inside `ToggleLineNumbers` would rebuild
    `_visibleLines` with the old `_viewportWidth` before `EditorView` has subtracted the
    new `GutterWidth`, producing an incorrect intermediate layout.

---

### Rendering Layer

- [x] **LN-03 — Add gutter color scheme to `EditorColorSchemes`**
  - Add a static `Attribute Gutter` (or `ColorScheme Gutter`) property using a muted
    foreground (e.g., `Color.Gray`) on the normal editor background, visually separating
    the gutter from the text area.

- [x] **LN-04 — Update `TerminalGuiRenderer` to render the gutter**
  - Cache `GutterWidth` from the `EditorViewModel` during `BeginFrame`.
  - In `BeginFrame`: paint the gutter column(s) with the gutter color attribute before
    clearing the text area; reduce the effective `Width` by `GutterWidth` and shift the
    text start column (`AbsX + GutterWidth`).
  - In `RenderLine`: render the right-aligned line number string (padded to `GutterWidth - 1`
    chars, followed by a `?` separator character) with the gutter attribute in the gutter
    area, then render the line text starting at `AbsX + GutterWidth`. For word-wrap
    continuation segments (where `line.SegmentStartColumn > 0`), render blank spaces in
    the gutter instead of a number. The `?` separator gives a visual boundary without
    consuming an extra column.
  - Apply current-line highlighting: when `line.DocumentLineIndex` equals
    `Session.Caret.Line`, render the line number using the normal editor foreground
    (`Color.Gray`) instead of the muted gutter foreground so the active line stands out.
  - Ensure the clipped text width is also reduced by `GutterWidth` so text does not
    overflow into the scrollbar column.

- [x] **LN-05 — Update `EditorView` to account for gutter width**
- Subscribe to `_viewModel.LineNumbersChanged` in the constructor; unsubscribe in `Dispose`.
- In the `LineNumbersChanged` handler call `SetNeedsDisplay()` and update the viewport
  width (call `UpdateViewportSize()` equivalent) so the ViewModel knows the effective
  text area width.
- In `UpdateViewportSize` (or wherever viewport width is set) subtract
  `_viewModel.GutterWidth` from the available `Frame.Width` before passing it to
  `_viewModel.SetViewportWidth(...)`.
- In `GetCaretScreenPosition` (or equivalent) add `_viewModel.GutterWidth` to the
  returned `screenX` so the cursor lands in the text area, not the gutter.
- In `MouseEvent`, apply the following gutter isolation logic before forwarding to
  `MouseBindings.TryExecute`. This must not change behaviour when line numbers are off
  (`GutterWidth == 0`):
  - **Wheel scroll events** (`WheeledUp` / `WheeledDown`): always pass through to
    `MouseBindings.TryExecute` unchanged — `ScrollWheel` uses no coordinates.
  - **Drag events** (`Button1Pressed + ReportMousePosition`) when `me.X < GutterWidth`:
    pass through with `me.X` adjusted by `-GutterWidth` (result is negative). The
    negative value is already handled by `CalculateColumn`'s left-edge path, which
    clamps the column and triggers edge-scroll-left — the correct behaviour when the
    user drags a selection back into the gutter region.
  - **All other button events** (`Click`, `DoubleClick`, `TripleClick`, `BeginSelection`,
    `EndSelection`) when `me.X < GutterWidth`: consume the event (return `true`) without
    calling `MouseBindings.TryExecute`. This prevents the gutter from moving the caret,
    starting selections, or triggering word/line selection.
  - **All events with `me.X >= GutterWidth`**: subtract `GutterWidth` from `me.X`
    before passing to `MouseBindings.TryExecute` so that all coordinate calculations
    (`CalculateColumn`, `ToDocumentPositionWrapped`, `EdgeScrollHorizontal`) operate
    in text-area-relative coordinates.

---

### Command and Menu Layer

- [x] **LN-06 — Create `LineNumbersCommand`**
  - Add `src/D20Tek.Notepad.Tui/Commands/LineNumbersCommand.cs` following the same pattern
    as `WordWrapCommand` and `StatusBarCommand`.
  - `CommandName = "ToggleLineNumbers"`.
  - `Execute` calls `viewModel.ToggleLineNumbers()`.
  - `Create` returns a `UiCommand`.

- [x] **LN-07 — Register command and add View menu entry**
  - In `CommandRegistry` (or wherever commands are registered, e.g., `EditorViewFactory`
    or the application setup code), register `LineNumbersCommand.Create(viewModel)`.
  - In `MenuDefinitions.Get(...)` add a checkable `_Line Numbers` entry to the `_View`
    menu using `LineNumbersCommand.CommandName` and
    `IsChecked: () => viewModel.IsLineNumbersEnabled`, placed after the `_Word Wrap` entry.

---

### Unit Tests

- [x] **LN-08 — Update `EditorSettingsTests` for the new property**
  - In `tests/D20Tek.Notepad.ViewModel.UnitTests/EditorSettingsTests.cs` add or update:
    - Constructor test verifies `LineNumbersEnabled` defaults to `false`.
    - `With` test includes `LineNumbersEnabled` in the changed-all-properties assertion.
    - Equality tests cover `LineNumbersEnabled` difference producing non-equal records.

- [x] **LN-09 — Create `EditorViewModelTests.LineNumbers.cs`**
  - Add `tests/D20Tek.Notepad.ViewModel.UnitTests/EditorViewModelTests.LineNumbers.cs`.
  - Test `IsLineNumbersEnabled` returns `false` with default settings.
  - Test `IsLineNumbersEnabled` returns `true` when `LineNumbersEnabled = true` in settings.
  - Test `ToggleLineNumbers` when disabled enables line numbers and updates `Settings`.
  - Test `ToggleLineNumbers` when enabled disables line numbers and updates `Settings`.
  - Test `ToggleLineNumbers` fires `LineNumbersChanged` event with the new value.
  - Test `GutterWidth` returns `0` when line numbers are disabled.
  - Test `GutterWidth` returns correct digit-count-plus-one for single-digit line count (e.g., 5 lines ? `2`).
  - Test `GutterWidth` returns correct width for multi-digit line counts (e.g., 100 lines ? `4`).

---

### Application Wiring

- [x] **LN-11 — Wire `LineNumbersChanged` in `Program.cs`**
  - Subscribe to `viewModel.LineNumbersChanged` in `Program.cs` to persist settings,
    following the same pattern as `WordWrapChanged`:
    ```
    viewModel.LineNumbersChanged += (_) => settingsService.Save(viewModel.Settings);
    ```
  - No layout change is needed (unlike `StatusBarChanged`): `EditorView` size is
    unaffected by the gutter; only the internal viewport width and renderer offset change.

---

### Feature Tracking

- [x] **LN-10 — Update `plans/features.md`**
  - Move **Line numbers** from the *Pending* table into the *Implemented* table.
  - Add shortcut column value (`—`) and note `Optional gutter display, toggle via View menu`.
  - Strike through the P3 priority entry in the implementation priority section.
