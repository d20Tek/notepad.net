# Find, Replace, and Go to Line Feature Implementation

## Overview
Implement search and navigation functionality for the Notepad.Tui editor:
- **Find** (Ctrl+F): Search dialog to find text occurrences
- **Find Next** (F3): Jump to next occurrence
- **Find Previous** (Shift+F3): Jump to previous occurrence  
- **Replace** (Ctrl+H): Search and replace dialog
- **Go to Line** (Ctrl+G): Jump to a specific line number

---

## Architecture Design

### Search State Management
Create a `SearchService` in the Core layer to manage search state and operations:
- Current search term
- Search options (case sensitivity, wrap around)
- Find next/previous logic
- Replace logic

### UI Dialogs
Terminal.Gui provides `Dialog` class for modal dialogs:
- `FindDialog` - Text field for search term, checkboxes for options
- `ReplaceDialog` - Extends find with replacement field and buttons
- `GoToLineDialog` - Simple numeric input for line number

### Highlighting (Future Enhancement)
Match highlighting can be added later by tracking all match positions and rendering them with a different color scheme.

---

## Task Breakdown

### Phase 1: Core Search Service

#### Task 1.1: Create SearchResult Type
- [x] Create `SearchResult` record in Core project
  - `TextPosition Start` - Start position of match
  - `TextPosition End` - End position of match
  - `int Length` - Length of match
  - `bool Found` - Whether a match was found

#### Task 1.2: Create SearchOptions Type
- [x] Create `SearchOptions` record in Core project
  - `bool CaseSensitive` - Case-sensitive search (default: false)
  - `bool WrapAround` - Wrap to beginning/end (default: true)

#### Task 1.3: Create SearchService
- [x] Create `SearchService` class in Core project
  - `SearchResult FindNext(IDocument document, string searchTerm, TextPosition fromPosition, SearchOptions options)`
  - `SearchResult FindPrevious(IDocument document, string searchTerm, TextPosition fromPosition, SearchOptions options)`
  - Private helpers: `SearchForward()` and `SearchBackward()`

#### Task 1.4: Unit Tests for SearchService
- [x] Create `SearchServiceTests.cs` with 24 comprehensive tests:
  - `FindNext_WithMatchOnSameLine_ReturnsMatch`
  - `FindNext_WithMatchOnNextLine_ReturnsMatch`
  - `FindNext_WithNoMatch_ReturnsNotFound`
  - `FindNext_WithEmptySearchTerm_ReturnsNotFound`
  - `FindNext_FromMiddleOfLine_FindsMatchAfterPosition`
  - `FindNext_FromAfterLastMatch_WithWrapAround_WrapsToBeginning`
  - `FindNext_FromAfterLastMatch_WithoutWrapAround_ReturnsNotFound`
  - `FindNext_CaseSensitive_RespectsCase`
  - `FindNext_CaseInsensitive_IgnoresCase`
  - `FindNext_CaseSensitive_NoMatch_ReturnsNotFound`
  - `FindPrevious_WithMatch_ReturnsMatch`
  - `FindPrevious_WithMatchOnPreviousLine_ReturnsMatch`
  - `FindPrevious_WithNoMatch_ReturnsNotFound`
  - `FindPrevious_FromBeforeFirstMatch_WithWrapAround_WrapsToEnd`
  - `FindPrevious_FromBeforeFirstMatch_WithoutWrapAround_ReturnsNotFound`
  - `FindPrevious_WithEmptySearchTerm_ReturnsNotFound`
  - `FindPrevious_CaseSensitive_RespectsCase`
  - `SearchResult_NotFound_HasCorrectProperties`
  - `SearchResult_Match_HasCorrectLength`
  - `SearchOptions_Default_HasExpectedValues`
  - `FindNext_EmptyDocument_ReturnsNotFound`
  - `FindNext_PositionBeyondDocumentEnd_ClampsPosition`
  - `FindNext_MultipleMatchesOnSameLine_FindsFirst`
  - `FindPrevious_MultipleMatchesOnSameLine_FindsLast`
  - `FindPrevious_WrapAround_WrapsToEnd`
  - `ReplaceAll_ReplacesAllOccurrences`
  - `ReplaceAll_WithNoMatches_ReturnsZero`

---

### Phase 2: ViewModel Search Integration

#### Task 2.1: Add Search State to EditorViewModel
- [x] Create `EditorViewModel.Search.cs` partial class
- [x] Add search state fields:
  - `string? _lastSearchTerm`
  - `SearchOptions _searchOptions`
- [x] Add `SearchService` dependency (created internally)

#### Task 2.2: Add Find Methods to EditorViewModel
- [x] Add `FindNext(string searchTerm)` method
  - Calls `SearchService.FindNext()`
  - If found, moves caret to match and selects it
  - Stores search term for F3/Shift+F3
  - Returns bool indicating if found
- [x] Add `FindPrevious(string searchTerm)` method
  - Calls `SearchService.FindPrevious()`
  - If found, moves caret to match and selects it
  - Returns bool indicating if found
- [x] Add `FindNextFromCurrent()` method (for F3 - uses last search term)
- [x] Add `FindPreviousFromCurrent()` method (for Shift+F3)

#### Task 2.3: Add Replace Methods to EditorViewModel
- [x] Add `Replace(string searchTerm, string replacement)` method
  - If current selection matches search term, replace it
  - Then find next occurrence
- [x] Add `ReplaceAll(string searchTerm, string replacement)` method
  - Replaces all occurrences
  - Returns count of replacements
  - Marks document dirty

#### Task 2.4: Add GoToLine Method to EditorViewModel
- [x] Add `GoToLine(int lineNumber)` method
  - Validate line number (1-based input, convert to 0-based)
  - Move caret to start of specified line
  - Ensure caret is visible
  - Clear selection

#### Task 2.5: Add Search Properties to EditorViewModel
- [x] Add `LastSearchTerm` property
- [x] Add `HasLastSearch` property (for enabling F3/Shift+F3)
- [x] Add `SearchOptions` property (get/set for dialog state)

#### Task 2.6: Unit Tests for EditorViewModel Search
- [x] Create `EditorViewModelTests.Search.cs` with 29 tests:
  - `FindNext_WithMatch_SelectsMatch`
  - `FindNext_WithNoMatch_ReturnsFalse`
  - `FindNext_StoresLastSearchTerm`
  - `FindNext_WithEmptyTerm_ReturnsFalse`
  - `FindNext_WithNullTerm_ThrowsArgumentNullException`
  - `FindNextFromCurrent_UsesLastSearchTerm`
  - `FindNextFromCurrent_WithNoLastSearch_ReturnsFalse`
  - `FindPrevious_WithMatch_SelectsMatch`
  - `FindPrevious_WithNoMatch_ReturnsFalse`
  - `FindPreviousFromCurrent_WithNoLastSearch_ReturnsFalse`
  - `Replace_WithMatchingSelection_ReplacesAndFindsNext`
  - `Replace_WithNoSelection_FindsNext`
  - `Replace_WithNonMatchingSelection_FindsNext`
  - `Replace_WithNullSearchTerm_ThrowsArgumentNullException`
  - `Replace_WithNullReplacement_ThrowsArgumentNullException`
  - `ReplaceAll_ReplacesAllAndMarksDirty`
  - `ReplaceAll_WithNoMatches_ReturnsZero`
  - `ReplaceAll_WithEmptySearchTerm_ReturnsZero`
  - `ReplaceAll_StoresLastSearchTerm`
  - `GoToLine_ValidLine_MovesCaret`
  - `GoToLine_LineTooHigh_ClampsToLastLine`
  - `GoToLine_LineTooLow_ClampsToFirstLine`
  - `GoToLine_ClearsSelection`
  - `SearchOptions_DefaultValue_IsSearchOptionsDefault`
  - `SearchOptions_CanBeSet`
  - `SearchOptions_SetToNull_UsesDefault`
  - `FindNext_CaseSensitive_RespectsOption`
  - `HasLastSearch_InitiallyFalse`
  - `HasLastSearch_TrueAfterSearch`

---

### Phase 3: Terminal.Gui Dialogs

#### Task 3.1: Create FindDialog
- [x] Create `Dialogs\FindDialog.cs` in Tui project
  - TextField for search term
  - CheckBox for "Case sensitive"
  - CheckBox for "Wrap around"  
  - Buttons: "Find Next", "Find Previous", "Close"
  - Result: search term, options, action taken via `FindAction` enum
- [x] Dialog is modal, captures state and action for caller to process

#### Task 3.2: Create ReplaceDialog
- [x] Create `Dialogs\ReplaceDialog.cs` in Tui project
  - TextField for search term and replacement text
  - CheckBox for "Case sensitive" and "Wrap around"
  - Buttons: "Find Next", "Replace", "Replace All", "Close"
  - Result: search term, replacement, options, action taken via `ReplaceAction` enum

#### Task 3.3: Create GoToLineDialog
- [x] Create `Dialogs\GoToLineDialog.cs` in Tui project
  - TextField for line number
  - Label showing valid range (1 to LineCount)
  - Buttons: "Go To", "Cancel"
  - Validation: must be valid number in range

---


### Phase 4: UI Commands

#### Task 4.1: Create Find Command
- [x] Create `FindCommand` in `Commands` folder
  - `CommandName = "Find"`
  - Opens `FindDialog`, processes FindNext/FindPrevious actions
  - Shortcut: `Ctrl+F`

#### Task 4.2: Create FindNext Command
- [x] Create `FindNextCommand` in `Commands` folder
  - `CommandName = "FindNext"`
  - Calls `viewModel.FindNextFromCurrent()`
  - Shortcut: `F3`
  - Shows message if no previous search or not found

#### Task 4.3: Create FindPrevious Command
- [x] Create `FindPreviousCommand` in `Commands` folder
  - `CommandName = "FindPrevious"`
  - Calls `viewModel.FindPreviousFromCurrent()`
  - Shortcut: `Shift+F3`
  - Shows message if no previous search or not found

#### Task 4.4: Create Replace Command
- [x] Create `ReplaceCommand` in `Commands` folder
  - `CommandName = "Replace"`
  - Opens `ReplaceDialog`, processes FindNext/Replace/ReplaceAll actions
  - Shortcut: `Ctrl+H`

#### Task 4.5: Create GoToLine Command
- [x] Create `GoToLineCommand` in `Commands` folder
  - `CommandName = "GoToLine"`
  - Opens `GoToLineDialog`
  - Shortcut: `Ctrl+G`


---

### Phase 5: Menu and Key Binding Integration

#### Task 5.1: Add Commands to KeyBindings
- [x] Add `F3` → `FindNextCommand.Execute(vm)`
- [x] Add `Shift+F3` → `FindPreviousCommand.Execute(vm)`
- [x] Add `Ctrl+F` → `FindCommand.Execute(vm)` (also in menu)
- [x] Add `Ctrl+H` → `ReplaceCommand.Execute(vm)` (also in menu)
- [x] Add `Ctrl+G` → `GoToLineCommand.Execute(vm)` (also in menu)

#### Task 5.2: Register Commands in MenuBuilder
- [x] Register `FindCommand`, `FindNextCommand`, `FindPreviousCommand`
- [x] Register `ReplaceCommand`, `GoToLineCommand`

#### Task 5.3: Add Menu Items to Edit Menu
- [x] Add separator after Select All
- [x] Add "Find..." (Ctrl+F) with `canExecute: always enabled`
- [x] Add "Find Next" (F3) with `canExecute: () => viewModel.HasLastSearch`
- [x] Add "Find Previous" (Shift+F3) with `canExecute: () => viewModel.HasLastSearch`
- [x] Add "Replace..." (Ctrl+H)
- [x] Add separator
- [x] Add "Go to Line..." (Ctrl+G)

---

## File Changes Summary

### New Files - Core Layer
| File | Purpose |
|------|---------|
| `src\D20Tek.Notepad.Core\Search\SearchResult.cs` | Search result record |
| `src\D20Tek.Notepad.Core\Search\SearchOptions.cs` | Search options record |
| `src\D20Tek.Notepad.Core\Search\SearchService.cs` | Search logic implementation |
| `tests\D20Tek.Notepad.Core.UnitTests\Search\SearchServiceTests.cs` | Search service tests |

### New Files - ViewModel Layer
| File | Purpose |
|------|---------|
| `src\D20Tek.Notepad.ViewModel\EditorViewModel.Search.cs` | Search methods partial class |
| `tests\D20Tek.Notepad.ViewModel.UnitTests\EditorViewModelTests.Search.cs` | Search tests |

### New Files - Tui Layer
| File | Purpose |
|------|---------|
| `src\D20Tek.Notepad.Tui\Dialogs\FindDialog.cs` | Find dialog |
| `src\D20Tek.Notepad.Tui\Dialogs\ReplaceDialog.cs` | Replace dialog |
| `src\D20Tek.Notepad.Tui\Dialogs\GoToLineDialog.cs` | Go to line dialog |
| `src\D20Tek.Notepad.Tui\Commands\SearchCommands.cs` | Find/Replace/GoTo commands |

### Modified Files
| File | Changes |
|------|---------|
| `src\D20Tek.Notepad.Tui\Input\KeyBindings.cs` | Add F3, Shift+F3, Ctrl+F, Ctrl+H, Ctrl+G bindings |
| `src\D20Tek.Notepad.Tui\Menus\MenuBuilder.cs` | Register search commands, add menu items |

---

## Edit Menu Layout (After Implementation)

```
Edit
??? Undo              Ctrl+Z
??? Redo              Ctrl+Y
??? ?????????????????
??? Cut               Ctrl+X
??? Copy              Ctrl+C
??? Paste             Ctrl+V
??? ?????????????????
??? Select All        Ctrl+A
??? ?????????????????
??? Find...           Ctrl+F
??? Find Next         F3          [disabled when no last search]
??? Find Previous     Shift+F3    [disabled when no last search]
??? Replace...        Ctrl+H
??? ?????????????????
??? Go to Line...     Ctrl+G
```

---

## Technical Notes

### Terminal.Gui Dialog Pattern
```csharp
var dialog = new Dialog("Find", 50, 10);
var searchField = new TextField("") { X = 1, Y = 1, Width = Dim.Fill() - 2 };
var findButton = new Button("Find Next") { X = 1, Y = 3 };

findButton.Clicked += () => {
    // Perform search
    Application.RequestStop();
};

dialog.Add(searchField, findButton);
Application.Run(dialog);
```

### Non-Modal Dialog Approach
For better UX, Find/Replace dialogs should ideally be non-modal (user can interact with editor while dialog is open). This may require:
- Using `Window` instead of `Dialog`
- Managing focus between dialog and editor
- Keeping dialog visible while searching

### Search Algorithm
For each line starting from current position:
1. Search within current line from current column
2. If not found, search from column 0 in subsequent lines
3. If wrap-around enabled and end reached, continue from document start
4. Stop when reaching original position (full cycle)

### Replace with Selection Validation
Before replacing, verify that current selection exactly matches the search term (considering case sensitivity). This prevents accidental replacements.

---

## Acceptance Criteria

1. **Find (Ctrl+F)**
   - [x] Opens find dialog with search term field
   - [x] "Find Next" finds and selects next occurrence
   - [x] "Find Previous" finds and selects previous occurrence
   - [x] Case sensitivity option works correctly
   - [x] Wrap around option works correctly
   - [x] Shows message when not found

2. **Find Next (F3)**
   - [x] Uses last search term
   - [x] Disabled when no previous search
   - [x] Wraps to beginning when reaching end

3. **Find Previous (Shift+F3)**
   - [x] Uses last search term
   - [x] Disabled when no previous search
   - [x] Wraps to end when reaching beginning

4. **Replace (Ctrl+H)**
- [x] Opens replace dialog
- [x] "Replace" replaces current selection if it matches, then finds next
- [x] "Replace All" replaces all occurrences and shows count
- [x] Replace operations are undoable
- [x] Replace all operation should be one undo step, not multiple

5. **Go to Line (Ctrl+G)**
   - [x] Opens dialog with line number input
   - [x] Shows valid range (1 to LineCount)
   - [x] Moves caret to specified line
   - [x] Invalid input is handled gracefully

---

## Future Enhancements (Out of Scope)

- **Highlight All Matches**: Show all matches with background highlighting
- **Incremental Search**: Highlight matches as user types in search field
- **Regex Search**: Support regular expression patterns
