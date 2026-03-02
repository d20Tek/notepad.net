# Navigation Enhancements Implementation

## Overview
Implement word-level navigation, selection, and deletion for the Notepad.Tui editor:
- **Word navigation left** (Ctrl+Left): Move caret to previous word boundary
- **Word navigation right** (Ctrl+Right): Move caret to next word boundary
- **Word selection left** (Ctrl+Shift+Left): Extend selection to previous word boundary
- **Word selection right** (Ctrl+Shift+Right): Extend selection to next word boundary
- **Delete word left** (Ctrl+Backspace): Delete from caret to previous word boundary
- **Delete word right** (Ctrl+Delete): Delete from caret to next word boundary

---

## Architecture Design

### Word Boundary Rules
A word boundary is the transition between word characters and non-word characters:
- **Word characters**: Letters, digits, and underscore (`char.IsLetterOrDigit(c) || c == '_'`)
- **Non-word characters**: Spaces, tabs, punctuation, operators, etc.
- Matches the existing `IsWordChar` in `EditorViewModel.Selection.cs` (used by double-click word selection)

### Word Boundary Algorithm
**Move to previous word boundary (Ctrl+Left):**
1. If at column 0, move to end of previous line
2. Skip any non-word characters backward (whitespace/punctuation)
3. Skip word characters backward until reaching a non-word character or line start

**Move to next word boundary (Ctrl+Right):**
1. If at end of line, move to start of next line
2. Skip word characters forward until reaching a non-word character
3. Skip any non-word characters forward (whitespace/punctuation)

### Layer Responsibilities
- **Core (`CaretNavigator`)**: Word boundary position computation
- **Core (`EditorCommandService`)**: Delete word operations
- **ViewModel (`EditorViewModel`)**: Delegate methods with viewport awareness
- **Tui (`KeyBindings`)**: Key binding registration

---

## Task Breakdown


### Phase 1: Core Word Boundary Logic

#### Task 1.1: Add Word Boundary Helpers to CaretNavigator
- [x] Add `private static bool IsWordChar(char c)` helper method
- [x] Add `private TextPosition ComputeWordLeft()` method
  - Skip non-word characters backward, then skip word characters backward
  - Cross line boundary to end of previous line when at column 0
- [x] Add `private TextPosition ComputeWordRight()` method
  - Skip word characters forward, then skip non-word characters forward
  - Cross line boundary to start of next line when at end of line

#### Task 1.2: Add Word Navigation Methods to CaretNavigator
- [x] Add `public void MoveWordLeft()` method
  - Calls `Move(ComputeWordLeft, resetAnchor: true)`
- [x] Add `public void MoveWordRight()` method
  - Calls `Move(ComputeWordRight, resetAnchor: true)`

#### Task 1.3: Add Word Selection Methods to CaretNavigator
- [x] Add `public void ExtendWordLeft()` method
  - Calls `Move(ComputeWordLeft, resetAnchor: false)`
- [x] Add `public void ExtendWordRight()` method
  - Calls `Move(ComputeWordRight, resetAnchor: false)`

#### Task 1.4: Unit Tests for CaretNavigator Word Methods
- [x] Create `CaretNavigatorWordTests.cs` with tests:
  - `MoveWordLeft_FromMiddleOfWord_MovesToWordStart`
  - `MoveWordLeft_FromWordStart_MovesToPreviousWordStart`
  - `MoveWordLeft_SkipsWhitespace`
  - `MoveWordLeft_SkipsPunctuation`
  - `MoveWordLeft_AtLineStart_MovesToEndOfPreviousLine`
  - `MoveWordLeft_AtDocumentStart_StaysAtStart`
  - `MoveWordLeft_MultiplePunctuation_SkipsAll`
  - `MoveWordRight_FromMiddleOfWord_MovesToNextWordStart`
  - `MoveWordRight_SkipsWhitespace`
  - `MoveWordRight_SkipsPunctuation`
  - `MoveWordRight_AtLineEnd_MovesToStartOfNextLine`
  - `MoveWordRight_AtDocumentEnd_StaysAtEnd`
  - `MoveWordRight_MultiplePunctuation_SkipsAll`
  - `ExtendWordLeft_ExtendsSelectionToWordStart`
  - `ExtendWordRight_ExtendsSelectionToNextWordStart`

---

### Phase 2: Core Delete Word Operations

#### Task 2.1: Add Delete Word Methods to EditorCommandService
- [x] Add `public void DeleteWordLeft()` method
  - Compute word-left position from current caret
  - Set anchor to caret, caret to word-left position (or vice versa)
  - Delete the selection
- [x] Add `public void DeleteWordRight()` method
  - Compute word-right position from current caret
  - Set anchor to caret, caret to word-right position (or vice versa)
  - Delete the selection

#### Task 2.2: Unit Tests for Delete Word Operations
- [x] Add tests to `EditorCommandServiceTests.Editing.cs`:
  - `DeleteWordLeft_FromMiddleOfWord_DeletesWordStart`
  - `DeleteWordLeft_FromWordStart_DeletesPreviousWord`
  - `DeleteWordLeft_AtDocumentStart_DoesNothing`
  - `DeleteWordLeft_WithSelection_DeletesSelection`
  - `DeleteWordRight_FromMiddleOfWord_DeletesToNextWordStart`
  - `DeleteWordRight_FromWordStart_DeletesCurrentWord`
  - `DeleteWordRight_AtDocumentEnd_DoesNothing`
  - `DeleteWordRight_WithSelection_DeletesSelection`

---

### Phase 3: ViewModel Integration

#### Task 3.1: Add Word Navigation Methods to EditorViewModel
- [x] Add to `EditorViewModel.Navigation.cs`:
  - `public void MoveWordLeft()`
  - `public void MoveWordRight()`
  - Each calls `EndTypingGroupIfNeeded()`, delegates to `Session.Navigator`, calls `EnsureCaretVisible()`

#### Task 3.2: Add Word Selection Methods to EditorViewModel
- [x] Add to `EditorViewModel.Navigation.cs`:
  - `public void ExtendWordLeft()`
  - `public void ExtendWordRight()`
  - Each calls `EndTypingGroupIfNeeded()`, delegates to `Session.Navigator`, calls `EnsureCaretVisible()`

#### Task 3.3: Add Delete Word Methods to EditorViewModel
- [x] Add to `EditorViewModel.Editing.cs`:
  - `public void DeleteWordLeft()`
  - `public void DeleteWordRight()`
  - Each calls `EndTypingGroupIfNeeded()`, delegates to `Commands`, calls `CheckDirtyStateChanged()`, `Refresh()`, `EnsureCaretVisible()`

#### Task 3.4: Unit Tests for ViewModel Methods
- [x] Add to `EditorViewModelTests.Navigation.cs`:
  - `MoveWordLeft_MovesCaretToWordBoundary`
  - `MoveWordRight_MovesCaretToWordBoundary`
  - `ExtendWordLeft_ExtendsSelectionToWordBoundary`
  - `ExtendWordRight_ExtendsSelectionToWordBoundary`
- [x] Add to `EditorViewModelTests.Editing.cs`:
  - `DeleteWordLeft_DeletesWord`
  - `DeleteWordRight_DeletesWord`

---

### Phase 4: Key Binding Integration

#### Task 4.1: Add Key Bindings
- [ ] Add to `KeyBindings.cs`:
  - `Ctrl+Left` ? `vm.MoveWordLeft()`
  - `Ctrl+Right` ? `vm.MoveWordRight()`
  - `Ctrl+Shift+Left` ? `vm.ExtendWordLeft()`
  - `Ctrl+Shift+Right` ? `vm.ExtendWordRight()`
  - `Ctrl+Backspace` ? `vm.DeleteWordLeft()`
  - `Ctrl+Delete` ? `vm.DeleteWordRight()`

#### Task 4.2: Build and Verify
- [ ] Build solution
- [ ] Run all tests
- [ ] Update features.md with completed items

---

## File Changes Summary

### Modified Files - Core Layer
| File | Changes |
|------|---------|
| `src\D20Tek.Notepad.Core\Editing\CaretNavigator.cs` | Add word boundary helpers, word navigation, word selection methods |
| `src\D20Tek.Notepad.Core\Editing\EditorCommandService.cs` | Add DeleteWordLeft, DeleteWordRight methods |

### Modified Files - ViewModel Layer
| File | Changes |
|------|---------|
| `src\D20Tek.Notepad.ViewModel\EditorViewModel.Navigation.cs` | Add MoveWordLeft/Right, ExtendWordLeft/Right |
| `src\D20Tek.Notepad.ViewModel\EditorViewModel.Editing.cs` | Add DeleteWordLeft, DeleteWordRight |

### Modified Files - Tui Layer
| File | Changes |
|------|---------|
| `src\D20Tek.Notepad.Tui\Input\KeyBindings.cs` | Add 6 key bindings |

### New Test Files
| File | Purpose |
|------|---------|
| `tests\D20Tek.Notepad.Core.UnitTests\Editing\CaretNavigatorWordTests.cs` | Word navigation and selection tests |

### Modified Test Files
| File | Changes |
|------|---------|
| `tests\D20Tek.Notepad.Core.UnitTests\Editing\EditorCommandServiceTests.cs` | Delete word tests |
| `tests\D20Tek.Notepad.ViewModel.UnitTests\EditorViewModelTests.Navigation.cs` | Word navigation tests |
| `tests\D20Tek.Notepad.ViewModel.UnitTests\EditorViewModelTests.Editing.cs` | Delete word tests |

---

## Word Boundary Examples

### Ctrl+Left (Move Word Left)
```
"Hello World|"  ?  "Hello |World"  ?  "|Hello World"
"foo.bar|"      ?  "foo.|bar"      ?  "foo|.bar"      ?  "|foo.bar"
"  Hello|"      ?  "  |Hello"      ?  "|  Hello"
```

### Ctrl+Right (Move Word Right)
```
"|Hello World"  ?  "Hello| World"  ?  "Hello World|"
"|foo.bar"      ?  "foo|.bar"      ?  "foo.|bar"      ?  "foo.bar|"
"|Hello  "      ?  "Hello|  "      ?  "Hello  |"
```

### Ctrl+Backspace (Delete Word Left)
```
"Hello World|"  ?  "Hello |"
"Hello   |"     ?  "|"  (deletes whitespace and previous word)
```

### Ctrl+Delete (Delete Word Right)
```
"|Hello World"  ?  "| World"
"|   Hello"     ?  "|"  (deletes whitespace and next word)
```

---

## Technical Notes

### Reusing IsWordChar
The `IsWordChar` helper already exists in `EditorViewModel.Selection.cs` for double-click word selection.
For Phase 1, a matching `IsWordChar` will be added to `CaretNavigator`. If a shared location is
preferred later, it can be extracted to a `WordHelper` utility class in the Core project.

### Delete Word with Existing Selection
When `DeleteWordLeft` or `DeleteWordRight` is called while text is selected, the standard
behavior (matching VS Code, Notepad++, etc.) is to delete just the selected text, ignoring the
word boundary logic. This is the same behavior as regular Backspace/Delete with a selection.

### Cross-Line Behavior
- **Ctrl+Left at column 0**: Moves to end of previous line (same as regular Left arrow behavior)
- **Ctrl+Right at end of line**: Moves to start of next line (same as regular Right arrow behavior)
- **Ctrl+Backspace at column 0**: Joins with previous line (same as regular Backspace)
- **Ctrl+Delete at end of line**: Joins with next line (same as regular Delete)

---

## Acceptance Criteria

1. **Word Navigation Left (Ctrl+Left)**
   - [ ] Moves caret to start of current word
   - [ ] Skips punctuation and whitespace
   - [ ] Crosses line boundaries
   - [ ] Stops at document start

2. **Word Navigation Right (Ctrl+Right)**
   - [ ] Moves caret to start of next word
   - [ ] Skips punctuation and whitespace
   - [ ] Crosses line boundaries
   - [ ] Stops at document end

3. **Word Selection Left (Ctrl+Shift+Left)**
   - [ ] Extends selection to previous word boundary
   - [ ] Preserves anchor position

4. **Word Selection Right (Ctrl+Shift+Right)**
   - [ ] Extends selection to next word boundary
   - [ ] Preserves anchor position

5. **Delete Word Left (Ctrl+Backspace)**
   - [ ] Deletes from caret to previous word boundary
   - [ ] With selection, deletes selection only
   - [ ] Undoable

6. **Delete Word Right (Ctrl+Delete)**
   - [ ] Deletes from caret to next word boundary
   - [ ] With selection, deletes selection only
   - [ ] Undoable
