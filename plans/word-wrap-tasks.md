# Word Wrap Feature Implementation

## Overview
Implement word wrap capability that behaves like Windows Notepad:
- When enabled, long lines wrap at the viewport width boundary
- Horizontal scrolling is disabled when word wrap is on
- Caret navigation respects wrapped lines visually
- Toggle via Format > Word Wrap menu item

---

## Task Breakdown

### Phase 1: Core Infrastructure (D20Tek.Notepad.ViewModel)

#### Task 1.1: Add WordWrap Setting to EditorSettings
- [x] Add `bool WordWrapEnabled { get; init; } = false;` property to `EditorSettings`
- [x] Update `EditorSettings.Default` if needed (not needed - uses default record initialization)

#### Task 1.2: Create WordWrapCalculator Utility
- [x] Create new class `WordWrapCalculator` in ViewModel project
- [x] Implement `WrapLine(string text, int viewportWidth)` method
  - Returns `List<WrappedSegment>` containing start index, length, and text for each segment
- [x] Handle edge cases: empty lines, lines shorter than viewport, single words longer than viewport
- [x] Wrap at word boundaries when possible (like Notepad), fall back to character wrap for long words

#### Task 1.3: Create WrappedLine Model
- [x] Create `WrappedSegment` record: `(int StartColumn, int Length, string Text)`
- [x] This maps each visual line segment back to its source column offset

#### Task 1.4: Update VisibleLinesBuilder for Word Wrap
- [x] Modify `Build()` method to check `EditorSettings.WordWrapEnabled`
- [x] When word wrap enabled:
  - Iterate document lines within viewport range
  - Call `WordWrapCalculator.WrapLine()` for each line
  - Produce multiple `ViewLine` entries per document line as needed
  - Track document-line-to-view-line mapping
- [x] Update `ViewLine` to include `DocumentLineIndex` and `SegmentStartColumn` for mapping

#### Task 1.5: Update Viewport for Word Wrap Mode
- [x] Add method `GetWrappedLineCount(IDocument document, int viewportWidth)` to calculate total visual lines
- [x] When word wrap enabled, disable horizontal scrolling (`HorizontalOffset` stays 0)
- [x] Update `EnsureLineVisible` to account for wrapped lines

---

### Phase 2: Caret & Selection Mapping (D20Tek.Notepad.ViewModel)

#### Task 2.1: Update ViewMapping for Wrapped Lines
- [x] Create mapping functions to convert between document position and wrapped view position
- [x] `ToWrappedViewPosition(TextPosition docPos, List<ViewLine> wrappedLines)`
- [x] `FromWrappedViewPosition(ViewPosition viewPos, List<ViewLine> wrappedLines)`

#### Task 2.2: Update EditorViewModel.Navigation
- [x] MoveUp/MoveDown should move by visual (wrapped) lines when word wrap enabled
- [x] MoveToLineStart/End should respect wrapped line boundaries OR document line (match Notepad behavior: goes to document line start/end)
- [x] Home key behavior: first press goes to wrapped line start, second to document line start

#### Task 2.3: Update Selection Mapping
- [x] Modify `GetSelectionSegmentForLine` to handle wrapped segments
- [x] Selection should paint correctly across wrapped line segments

---

### Phase 3: Settings & State Management (D20Tek.Notepad.ViewModel)

#### Task 3.1: Add WordWrap Toggle to EditorViewModel
- [x] Add `bool IsWordWrapEnabled` property
- [x] Add `ToggleWordWrap()` method that:
  - Toggles the setting
  - Resets horizontal scroll to 0
  - Triggers `Refresh()` to rebuild visible lines

#### Task 3.2: Persist Word Wrap Setting
- [x] Consider persisting setting across sessions (app settings/config)
  - Persist file next to the tui exe as json (editor-settings.json) in a SettingsService class
  - Save and load the full EditorSettings class to that file
  - Load the EditorSettings on startup

---

### Phase 4: UI Integration (D20Tek.Notepad.Tui)

#### Task 4.1: Add View Menu with Word Wrap Toggle
- [x] Update `MenuBuilder` to add "View" menu
- [x] Add "Word Wrap" menu item with checkmark indicator
- [x] Wire menu item to `EditorViewModel.ToggleWordWrap()`
- [x] Menu item should show checked state when word wrap is enabled

#### Task 4.2: Create WordWrapCommand
- [x] Create `WordWrapCommand` class implementing the command pattern
- [x] Register command in `CommandRegistry`

#### Task 4.3: Update TerminalGuiRenderer (if needed)
- [x] Ensure renderer handles variable-length wrapped lines correctly (verified - already works)
- [x] Verify caret positioning works with wrapped content (verified - MapCaret handles word wrap)

#### Task 4.4: Disable Horizontal Scrollbar When Word Wrap Enabled
- [x] Hide or disable horizontal scroll indicator in word wrap mode

---

### Phase 5: Unit Tests

#### Task 5.1: WordWrapCalculator Tests
- [x] Test empty string returns single empty segment
- [x] Test line shorter than viewport returns single segment
- [x] Test line exactly viewport width returns single segment
- [x] Test line longer than viewport wraps at word boundary
- [x] Test long word exceeding viewport wraps at character boundary
- [x] Test multiple spaces and whitespace handling
- [x] Test wrapping with various viewport widths

#### Task 5.1a: EditorSettings.WordWrapEnabled Tests
- [x] Test default value is false
- [x] Test constructor with custom WordWrapEnabled value

#### Task 5.1b: WrappedSegment Tests
- [x] Test constructor sets properties
- [x] Test with expression creates new instance with updates
- [x] Test equality with same values
- [x] Test inequality with different values

#### Task 5.2: VisibleLinesBuilder Word Wrap Tests
- [x] Test wrapped lines produced correctly
- [x] Test document line mapping preserved
- [x] Test viewport scrolling with wrapped content
- [x] Test SegmentStartColumn values correct
- [x] Test multiple document lines wrap correctly
- [x] Test viewport height limits wrapped lines
- [x] Test short lines return single segment
- [x] Test empty line returns one segment
- [x] Test zero viewport width returns empty list

#### Task 5.2a: ViewLine Tests
- [x] Test constructor with SegmentStartColumn sets all properties
- [x] Test constructor without SegmentStartColumn defaults to zero
- [x] Test negative SegmentStartColumn throws exception

#### Task 5.3: EditorViewModel Word Wrap Tests
- [x] Test `ToggleWordWrap()` changes state
- [x] Test horizontal offset resets when word wrap enabled
- [x] Test navigation in word wrap mode

---

## Acceptance Criteria
- [x] View > Word Wrap menu item toggles word wrap on/off
- [x] Menu item shows checkmark when word wrap is enabled
- [x] Long lines wrap at viewport boundary when enabled
- [x] Words wrap at word boundaries (spaces) when possible
- [x] Horizontal scrollbar is hidden/disabled when word wrap is on
- [x] Caret navigation (up/down arrows) moves by visual lines
- [x] Home/End keys navigate within document lines (Notepad behavior)
- [x] Selection highlighting works correctly across wrapped lines
- [ ] Performance is acceptable for large documents (needs manual testing)

---

## Dependencies
- `EditorSettings` (existing)
- `VisibleLinesBuilder` (existing - needs modification)
- `Viewport` (existing - needs modification)
- `ViewLine` (existing - may need extension)
- `MenuBuilder` (existing - needs Format menu)

---

## Notes
- Windows Notepad wraps at word boundaries, not character boundaries
- When a single word exceeds viewport width, it wraps mid-word
- Notepad's Home key goes to document line start, not wrapped line start
- Consider performance impact on large files with many long lines