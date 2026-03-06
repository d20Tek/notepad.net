# Status Bar Feature Implementation

## Overview
Implement a status bar at the bottom of the editor window that displays:
- **Line/Column position**: `Ln X, Col Y`
- **Document statistics**: Total lines, total characters
- **Encoding**: UTF-8, ASCII, etc.
- **Line endings**: CRLF, LF, CR

---

## Architecture Design

### Status Information Flow
The status bar needs real-time updates from the EditorViewModel:
- Caret position changes → Update Ln/Col
- Document changes → Update total lines/characters
- Document load → Update encoding and line endings

### Components
1. **EditorViewModel.Status.cs** - Exposes status properties and events
2. **StatusBarView** - Terminal.Gui StatusBar implementation
3. **MainWindow integration** - Wire up the status bar to the main window

---

## Task Breakdown

### Phase 1: ViewModel Status Properties

#### Task 1.1: Create Status Properties in EditorViewModel
- [x] Create `StatusDetails.cs` record with all status properties
- [x] Add `CurrentLine` property (1-based line number)
- [x] Add `CurrentColumn` property (1-based column number)
- [x] Add `TotalLines` property
- [x] Add `TotalCharacters` property
- [x] Add `DocumentEncoding` property (returns encoding name string)
- [x] Add `LineEndingStyle` property (returns "CRLF", "LF", or "CR")

#### Task 1.2: Add Status Changed Event
- [x] Add `StatusChanged` event to EditorViewModel that passes `StatusDetails` argument
- [x] Add `CurrentStatus` property exposing the cached status value
- [x] Fire event when caret moves
- [x] Fire event when document content changes
- [x] Fire event when document is loaded/replaced

#### Task 1.3: Unit Tests for Status Properties
- [x] Create `StatusDetailsTests.cs` with record type tests (Constructor, With, Equality, Empty)
- [x] Create `EditorViewModelTests.Status.cs` with tests:
  - `CurrentLine_ReturnsOneBasedLineNumber`
  - `CurrentColumn_ReturnsOneBasedColumnNumber`
  - `CurrentLine_UpdatesOnCaretMove`
  - `CurrentColumn_UpdatesOnCaretMove`
  - `TotalLines_ReturnsDocumentLineCount`
  - `TotalCharacters_ReturnsTotalCharacterCount`
  - `DocumentEncoding_ReturnsEncodingName`
  - `LineEndingStyle_ReturnsCRLF`
  - `LineEndingStyle_ReturnsLF`
  - `LineEndingStyle_ReturnsCR`
  - `StatusChanged_FiredOnCaretMove`
  - `StatusChanged_FiredOnDocumentChange`

---

### Phase 2: Status Bar UI Component

#### Task 2.1: Create StatusBarView
- [x] Create `StatusBarView.cs` in Tui project
  - Inherits from `View`, implements `IDisposable`
  - Constructor takes `EditorViewModel`, subscribes to `StatusChanged`
  - Anchored to bottom of window with `Pos.AnchorEnd(1)`

#### Task 2.2: Define Status Bar Layout
- [x] Left section: `Ln X, Col Y`
- [x] Center section: `Lines: X | Chars: X`
- [x] Right section: `UTF-8 CRLF`
- [x] Custom `Redraw` fills background then paints each section

#### Task 2.3: Status Bar Formatting
- [x] Format line/column: `Ln {line}, Col {col}`
- [x] Format statistics: `Lines: {total:N0} | Chars: {chars:N0}`
- [x] Format encoding: `{encoding} {lineEnding}`
- [x] Center section omitted gracefully when window too narrow

---

### Phase 3: Integration

#### Task 3.1: Update MainWindow
- [ ] Add `StatusBarView` to `MainWindow`
- [ ] Position status bar at bottom of window
- [ ] Ensure status bar doesn't overlap with editor content

#### Task 3.2: Wire Up Events
- [ ] Connect `EditorViewModel.StatusChanged` to status bar refresh
- [ ] Ensure status bar updates on:
  - Application startup
  - Caret movement
  - Text editing
  - File open
  - File new

#### Task 3.3: Adjust Editor Layout
- [ ] Reduce editor height by 1 to accommodate status bar
- [ ] Ensure scrollbar calculations account for status bar
- [ ] Test with various window sizes

---

### Phase 4: Polish and Edge Cases

#### Task 4.1: Performance Optimization
- [ ] Debounce status updates during rapid typing
- [ ] Only recalculate total characters when document changes
- [ ] Cache encoding and line ending (only update on document load)

#### Task 4.2: Edge Cases
- [ ] Handle empty document (show Ln 1, Col 1)
- [ ] Handle very long lines (column count accuracy)
- [ ] Handle unknown encoding gracefully
- [ ] Test with large documents

---

## File Changes Summary

### New Files
| File | Purpose |
|------|---------|
| `src\D20Tek.Notepad.ViewModel\StatusDetails.cs` | Status properties class |
| `src\D20Tek.Notepad.ViewModel\EditorViewModel.cs` | Status update event |
| `src\D20Tek.Notepad.Tui\StatusBarView.cs` | Status bar UI component |
| `tests\D20Tek.Notepad.ViewModel.UnitTests\StatusDetailsTests.cs` | Status property tests |

### Modified Files
| File | Changes |
|------|---------|
| `src\D20Tek.Notepad.Tui\MainWindow.cs` | Add StatusBarView, adjust layout |
| `src\D20Tek.Notepad.Tui\EditorView.cs` | Adjust height for status bar |

---

## Status Bar Layout

```
┌─────────────────────────────────────────────────────────────────┐
│ File  Edit  View                                                │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│                      Editor Content                             │
│                                                                 │
├─────────────────────────────────────────────────────────────────┤
│ Ln 42, Col 15          Lines: 156 | Chars: 4,521      UTF-8 CRLF│
└─────────────────────────────────────────────────────────────────┘
```

---

## Technical Notes

### Terminal.Gui StatusBar
```csharp
var statusBar = new StatusBar(new StatusItem[]
{
    new(Key.Null, "Ln 1, Col 1", null),
    new(Key.Null, "Lines: 1 | Chars: 0", null),
    new(Key.Null, "UTF-8 CRLF", null)
});
```

### Updating Status Items
StatusBar items are typically static, but we can:
1. Recreate the StatusBar on each update (simple but may flicker)
2. Update `StatusItem.Title` property directly
3. Use custom rendering with `Redraw`

### Character Count Calculation
```csharp
public int TotalCharacters => Session.Document.Lines
    .Sum(line => line.Content.Length) + 
    (Session.Document.LineCount - 1); // Add line ending characters
```

### Line Ending Display
```csharp
public string LineEndingStyleDisplay => Session.Document.LineEndings switch
{
    LineEndingStyle.CRLF => "CRLF",
    LineEndingStyle.LF => "LF",
    LineEndingStyle.CR => "CR",
    _ => "LF"
};
```

---

## Acceptance Criteria

1. **Line/Column Display**
   - [ ] Shows current line number (1-based)
   - [ ] Shows current column number (1-based)
   - [ ] Updates immediately on caret movement
   - [ ] Updates on keyboard navigation

2. **Document Statistics**
   - [ ] Shows total line count
   - [ ] Shows total character count
   - [ ] Updates on text insertion/deletion

3. **Encoding Display**
   - [ ] Shows document encoding (UTF-8, ASCII, etc.)
   - [ ] Updates when file is opened

4. **Line Ending Display**
   - [ ] Shows CRLF, LF, or CR
   - [ ] Updates when file is opened

5. **Visual Integration**
   - [ ] Status bar appears at bottom of window
   - [ ] Does not overlap editor content
   - [ ] Readable with proper spacing
   - [ ] Consistent with application color scheme

---

## Future Enhancements (Out of Scope)

- **Click to Go to Line**: Click on Ln/Col to open Go to Line dialog
- **Encoding selector**: Click to change encoding
- **Line ending selector**: Click to change line endings
- **Word count**: Additional statistic
- **Selection info**: Show selection length when text is selected
- **Insert/Overwrite mode indicator**: INS/OVR toggle

