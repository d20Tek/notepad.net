# File > Encoding Submenu Feature

## Overview
Add a `File > Encoding` submenu that lets users change the encoding of the current document.
The submenu shows a checkmark next to the active encoding and fires `StatusChanged` so the
status bar reflects the change immediately.

**Supported encodings**
| Menu label | Encoding instance |
|---|---|
| `UTF-_8` | `new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)` |
| `UTF-8 with _BOM` | `new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)` |
| `_Unicode (UTF-16 LE)` | `Encoding.Unicode` |
| `_BigEndian Unicode (UTF-16 BE)` | `Encoding.BigEndianUnicode` |

---

## Architecture Design

### Data flow
```
User clicks "UTF-8 with BOM"
  → ChangeEncodingCommand.Execute()
  → EditorViewModel.ChangeEncoding(new UTF8Encoding(true))
    → Session.Document.SetEncoding(encoding)   // marks document dirty
    → Refresh()
      → BuildStatus()                          // detects encoding ref changed
      → StatusChanged fires with new encoding string
  → Status bar redraws: "UTF-8 BOM CRLF"
```

### Encoding identity
Two encodings are the same selection when both `WebName` AND `GetPreamble()` match.
This correctly separates UTF-8 (no BOM) from UTF-8 with BOM, which share the same
`WebName` ("utf-8") but differ in their preamble bytes.

### Status cache invalidation
`EditorViewModel.Status.cs` currently caches `_cachedEncoding` keyed only on the
**document reference** (`ReferenceEquals`). Calling `SetEncoding()` replaces the
`Document.Encoding` property value on the *same* document instance, so the document
reference stays equal and the cached string becomes stale.

Fix: add a second cached field `_cachedDocumentEncoding` (type `Encoding?`) and
compare it by reference in `BuildStatus()`. When the encoding reference changes on
the same document, update `_cachedEncoding` without resetting the char-count cache.

### Save behaviour
`SimpleTextStorage.Save()` already writes via `new StreamWriter(stream, document.Encoding)`.
`StreamWriter` automatically emits the encoding's preamble bytes, so saving after a
BOM change produces the correct byte-order mark with no additional changes needed.

### Checkable submenu items — Terminal.Gui v1
`MenuBarItem` (which inherits `MenuItem`) nested inside another `MenuBarItem`'s
children becomes a flyout submenu in Terminal.Gui v1. Checkable items inside the
submenu use `MenuItemCheckStyle.Checked`. The existing `MenuBuilder.BuildCheckableItem`
handles this correctly; no Terminal.Gui changes are needed.

---

## Phase 1: Menu Abstraction Redesign

The current flat `MenuItemDefinition`/`MenuDefinition` types cannot represent submenus.
Replace them with a sealed record hierarchy before adding the `Encoding` submenu.

### Task 1.1: Create `MenuEntry.cs` — sealed record hierarchy
- [x] Define `abstract record MenuEntry` as base type
- [x] Add `static MenuEntry Separator { get; }` factory property (returns `SeparatorEntry`)
- [x] Define `sealed record SeparatorEntry() : MenuEntry`
- [x] Define `sealed record CommandEntry(string Label, string CommandName, bool IsCheckable,
       Func<bool>? IsChecked, Func<bool>? CanExecute) : MenuEntry`
  - All `Func<>` parameters default to `null`, `IsCheckable` defaults to `false`
- [x] Define `sealed record SubMenuEntry(string Label, IReadOnlyList<MenuEntry> Items) : MenuEntry`
  - Add convenience `public SubMenuEntry(string label, params MenuEntry[] items)` constructor
    that calls `this(label, (IReadOnlyList<MenuEntry>)items)`

### Task 1.2: Create `TopLevelMenu.cs`
- [x] Define `sealed record TopLevelMenu(string Title, IReadOnlyList<MenuEntry> Items)`
- [x] Add `params MenuEntry[]` convenience constructor calling `this(title, (IReadOnlyList<MenuEntry>)items)`
- [x] Replaces `MenuDefinition.cs` (which can then be deleted)

### Task 1.3: Update `MenuBuilder.cs`
- [x] Change `Build()` to accept `TopLevelMenu[]` from `MenuDefinitions.Get()`
- [x] Extract `BuildItems(IReadOnlyList<MenuEntry>, CommandRegistry)` helper returning `MenuItem?[]`
- [x] Replace `CreateMenuItem` if-chain with exhaustive switch expression:

  ```csharp
  private static MenuItem? BuildItem(MenuEntry entry, CommandRegistry commands) => entry switch
  {
      SeparatorEntry        => null,
      SubMenuEntry sub      => new MenuBarItem(sub.Label, BuildItems(sub.Items, commands)),
      CommandEntry cmd      => BuildCommandItem(cmd, commands),
      _                     => throw new UnreachableException($"Unhandled MenuEntry: {entry.GetType().Name}")
  };
  ```
- [x] Rename `CreateCheckableMenuItem` → `BuildCheckableItem` for consistency
- [x] Rename `CreateMenuItem` → `BuildCommandItem` for clarity

### Task 1.4: Update `MenuDefinitions.cs`
- [x] Change return type from `MenuDefinition[]` to `TopLevelMenu[]`
- [x] Replace all `new MenuDefinition(...)` with `new TopLevelMenu(...)`
- [x] Replace all `new MenuItemDefinition(...)` with `new CommandEntry(...)`
- [x] Replace all `MenuItemDefinition.Separator` with `MenuEntry.Separator`
- [x] Verify all existing menu items compile and behave identically

### Task 1.5: Delete old abstraction files
- [x] Delete `src\D20Tek.Notepad.Tui\Menus\MenuDefinition.cs`
- [x] Delete `src\D20Tek.Notepad.Tui\Menus\MenuItemDefinition.cs`
- [x] Confirm build is clean

---

## Phase 2: ViewModel Encoding Support

### Task 2.1: Fix `BuildStatus()` encoding cache invalidation
File: `src\D20Tek.Notepad.ViewModel\EditorViewModel.Status.cs`
- [x] Add `private Encoding? _cachedDocumentEncoding;` field
- [x] In `BuildStatus()`, split the existing document-change block into two checks:

  ```csharp
  bool documentChanged = !ReferenceEquals(_cachedDocument, Session.Document);

  if (documentChanged)
  {
      _cachedDocument = Session.Document;
      _cachedDocumentEncoding = Session.Document.Encoding;
      _cachedEncoding = GetEncodingDisplay(Session.Document.Encoding);
      _cachedLineEnding = GetLineEndingDisplay(Session.Document.LineEndingStyle);
      _cachedCharVersion = -1;
  }
  else if (!ReferenceEquals(_cachedDocumentEncoding, Session.Document.Encoding))
  {
      _cachedDocumentEncoding = Session.Document.Encoding;
      _cachedEncoding = GetEncodingDisplay(Session.Document.Encoding);
  }
  ```

### Task 2.2: Add `ChangeEncoding()` to `EditorViewModel`
File: `src\D20Tek.Notepad.ViewModel\EditorViewModel.cs`
- [x] Add `public void ChangeEncoding(Encoding encoding)` method
  - Calls `Session.Document.SetEncoding(encoding)`
  - Calls `Refresh()` (which invokes `BuildStatus()` → fires `StatusChanged` if encoding changed)
  [x] Marks the document as dirty via `SetEncoding()`, so no explicit dirty flag

### Task 2.3: Add `IsCurrentEncoding()` to `EditorViewModel`
File: `src\D20Tek.Notepad.ViewModel\EditorViewModel.cs`
- [x] Add `public bool IsCurrentEncoding(Encoding encoding)` method
  - Returns `true` when `Document.Encoding.WebName == encoding.WebName`
    AND `Document.Encoding.GetPreamble().SequenceEqual(encoding.GetPreamble())`
  - This correctly separates UTF-8 (no BOM) from UTF-8 with BOM

### Task 2.4: Unit tests for ViewModel encoding methods
File: `tests\D20Tek.Notepad.ViewModel.UnitTests\EditorViewModelTests.Encoding.cs`
- [x] `ChangeEncoding_Utf8_SetsDocumentEncoding`
- [x] `ChangeEncoding_Utf8Bom_SetsDocumentEncoding`
- [x] `ChangeEncoding_Unicode_SetsDocumentEncoding`
- [x] `ChangeEncoding_BigEndianUnicode_SetsDocumentEncoding`
- [x] `ChangeEncoding_FiresStatusChangedWithUpdatedEncoding`
- [x] `ChangeEncoding_MarksDocumentDirty` (asserts both `Document.IsModified` and `IsDirty`)
- [x] `IsCurrentEncoding_WhenEncodingMatches_ReturnsTrue`
- [x] `IsCurrentEncoding_WhenEncodingDiffers_ReturnsFalse`
- [x] `IsCurrentEncoding_Utf8VsUtf8Bom_AreDistinct`

---

## Phase 3: Encoding Command and Menu

### Task 3.1: Create `ChangeEncodingCommand.cs`
File: `src\D20Tek.Notepad.Tui\Commands\ChangeEncodingCommand.cs`
- [x] Define four `const string` command names:
  - `Utf8CommandName = "EncodingUtf8"`
  - `Utf8BomCommandName = "EncodingUtf8Bom"`
  - `UnicodeCommandName = "EncodingUnicode"`
  - `BigEndianUnicodeCommandName = "EncodingBigEndianUnicode"`
- [x] Add `CreateUtf8(viewModel)` → `new UiCommand(Utf8CommandName, () => viewModel.ChangeEncoding(new UTF8Encoding(false)))`
- [x] Add `CreateUtf8Bom(viewModel)` → `new UiCommand(Utf8BomCommandName, () => viewModel.ChangeEncoding(new UTF8Encoding(true)))`
- [x] Add `CreateUnicode(viewModel)` → `new UiCommand(UnicodeCommandName, () => viewModel.ChangeEncoding(Encoding.Unicode))`
- [x] Add `CreateBigEndianUnicode(viewModel)` → `new UiCommand(BigEndianUnicodeCommandName, () => viewModel.ChangeEncoding(Encoding.BigEndianUnicode))`

### Task 3.2: Register commands in `MenuCommands.cs`
- [x] Add a `// Encoding commands` section in `MenuCommands.Get()`
- [x] Register all four `ChangeEncodingCommand.Create*()` commands

### Task 3.3: Add `Encoding` submenu to `MenuDefinitions.cs`
- [x] Add a `new SubMenuEntry("_Encoding", ...)` in the `_File` `TopLevelMenu`, between `Save _As...` and `_Quit`:
  ```csharp
  new SubMenuEntry("_Encoding",
      new CommandEntry("UTF-_8", ChangeEncodingCommand.Utf8CommandName,
          isCheckable: true,
          isChecked: () => viewModel.IsCurrentEncoding(new UTF8Encoding(false))),
      new CommandEntry("UTF-8 with _BOM", ChangeEncodingCommand.Utf8BomCommandName,
          isCheckable: true,
          isChecked: () => viewModel.IsCurrentEncoding(new UTF8Encoding(true))),
      new CommandEntry("_Unicode (UTF-16 LE)", ChangeEncodingCommand.UnicodeCommandName,
          isCheckable: true,
          isChecked: () => viewModel.IsCurrentEncoding(Encoding.Unicode)),
      new CommandEntry("_BigEndian Unicode (UTF-16 BE)", ChangeEncodingCommand.BigEndianUnicodeCommandName,
          isCheckable: true,
          isChecked: () => viewModel.IsCurrentEncoding(Encoding.BigEndianUnicode)))
  ```

---

## File Changes Summary

### New Files
| File | Purpose |
|---|---|
| `src\D20Tek.Notepad.Tui\Menus\MenuEntry.cs` | Sealed record hierarchy: base + `SeparatorEntry` + `CommandEntry` + `SubMenuEntry` |
| `src\D20Tek.Notepad.Tui\Menus\TopLevelMenu.cs` | Replaces `MenuDefinition` |
| `src\D20Tek.Notepad.Tui\Commands\ChangeEncodingCommand.cs` | Four encoding commands |
| `tests\D20Tek.Notepad.ViewModel.UnitTests\EditorViewModelTests.Encoding.cs` | 9 ViewModel tests |

### Modified Files
| File | Change |
|---|---|
| `src\D20Tek.Notepad.Tui\Menus\MenuBuilder.cs` | Exhaustive switch, recursive `BuildItems()` |
| `src\D20Tek.Notepad.Tui\Menus\MenuDefinitions.cs` | Use `TopLevelMenu` / `CommandEntry` / `SubMenuEntry` |
| `src\D20Tek.Notepad.Tui\Menus\MenuCommands.cs` | Register 4 encoding commands |
| `src\D20Tek.Notepad.ViewModel\EditorViewModel.cs` | `ChangeEncoding()`, `IsCurrentEncoding()` |
| `src\D20Tek.Notepad.ViewModel\EditorViewModel.Status.cs` | `_cachedDocumentEncoding` field, split document-change check |

### Deleted Files
| File | Replaced by |
|---|---|
| `src\D20Tek.Notepad.Tui\Menus\MenuDefinition.cs` | `TopLevelMenu.cs` |
| `src\D20Tek.Notepad.Tui\Menus\MenuItemDefinition.cs` | `MenuEntry.cs` |

---

## Acceptance Criteria

- [x] File > Encoding submenu is visible in the menu bar
- [x] Submenu shows four encoding choices
- [x] Active encoding has a checkmark; others do not
- [x] Selecting a different encoding updates the checkmark immediately on next menu open
- [x] Status bar encoding section updates immediately after selection (if status bar is visible)
- [x] Changing encoding marks the document as dirty
- [x] Saving after an encoding change writes the file in the new encoding
- [x] Opening a UTF-8 with BOM file selects "UTF-8 with BOM" checkmark
- [x] Opening a plain UTF-8 file (no BOM) selects "UTF-8" checkmark
- [x] All existing menu items (File, Edit, View) continue to work identically after the redesign

---

## Technical Notes

### Why `GetPreamble()` for encoding identity
`UTF8Encoding(false).WebName` and `UTF8Encoding(true).WebName` are both `"utf-8"`.
`WebName` alone cannot distinguish them. `GetPreamble()` returns `[]` vs `[0xEF, 0xBB, 0xBF]`,
making `SequenceEqual` on preamble bytes the correct identity check.

### `StreamWriter` preamble handling
`new StreamWriter(stream, encoding)` automatically writes `encoding.GetPreamble()` at the
start of the stream. This is why `SimpleTextStorage.Save()` does not need any change —
the BOM bytes are handled by `StreamWriter` based on the encoding's preamble.

### Menu checkmark refresh timing
Terminal.Gui evaluates `MenuItem.Checked` at the time the menu is opened, not continuously.
The `isChecked: () => viewModel.IsCurrentEncoding(...)` lambda is called each time the
menu renders, so it always reflects the current document encoding.

### `UnreachableException` in `MenuBuilder`
The `_` throw arm in `BuildItem` guards against future `MenuEntry` subtypes being added
without a corresponding `BuildItem` case. It converts a silent miss into an immediate
runtime failure during development.
