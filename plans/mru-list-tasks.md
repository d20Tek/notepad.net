# File > Open Recent (MRU List) Feature

## Overview
Add a `File > Open Recent` submenu listing the 10 most recently used files.
The list is persisted in `editor-settings.json` via `EditorSettings`. When the
list is empty, the submenu shows a disabled `(Empty)` placeholder. Selecting an
entry opens the file with the same unsaved-prompt behaviour as `File > Open`.

**MRU update triggers**
| Action | Trigger |
|---|---|
| File > Open | `FilePathChanged` fires after `viewModel.SetFilePath()` |
| File > Save As | `FilePathChanged` fires after `FileSaveCommand.SaveToFile()` sets the path |
| File > Open Recent | `FilePathChanged` fires as part of the open flow |
| CLI file argument | `FilePathChanged` fires when `Program.cs` calls `viewModel.SetFilePath()` |

---

## Architecture Design

### Data flow
```
User opens a file (any trigger)
  → viewModel.SetFilePath(path)
    → FilePathChanged fires
      → AddToRecentFiles(path)
        → _settings updated with new MruList
        → RecentFilesChanged fires
          → Program.cs saves settings
          → Program.cs rebuilds and replaces MenuBar
```

### `MruList` design
- Immutable record; `Add()` returns a new instance.
- Deduplicates: if the path already exists it is moved to position 0.
- Capped at `MaxCapacity = 10`; oldest entry is dropped when full.
- Stored as `IReadOnlyList<string>` in `EditorSettings.RecentFiles`.

### Menu rebuild strategy
`MenuBuilder.Build()` is called once at startup and again whenever
`RecentFilesChanged` fires. `Program.cs` removes the old `MenuBar` from
`Application.Top`, builds a new one, and adds it back. This mirrors the
existing pattern where `WordWrapChanged` and `StatusBarChanged` already
trigger layout mutations on `Application.Top`.

### `ActionEntry` menu entry type
MRU items are not registered in `CommandRegistry` (the list is dynamic).
A new `sealed record ActionEntry(string Label, Action Execute, Func<bool>? CanExecute = null) : MenuEntry`
is added to the `MenuEntry` hierarchy. `MenuBuilder.BuildItem()` handles it
via a new switch arm, creating a plain `MenuItem` directly.

### Menu item labels
```
_1 first.txt
_2 second.txt
...
_9 ninth.txt
_0 tenth.txt
```
Numbers cycle 1–9, then 0 for the tenth. The underscore prefix provides
keyboard accelerator navigation consistent with the rest of the menu.
Full paths are stored internally in `RecentFiles` but only the filename is shown in the label.

---

## Phase 1: `MruList` Record

### Task 1.1: Create `MruList.cs`
File: `src\D20Tek.Notepad.ViewModel\MruList.cs`
- [x] Define `public sealed record MruList`
- [x] Add `public const int MaxCapacity = 10;`
- [x] Add `public static MruList Empty { get; } = new();`
- [x] Add `public IReadOnlyList<string> Paths { get; init; } = [];`
- [x] Add `public MruList Add(string path)`:
  - Validate `path` is not null or whitespace
  - Build new list: `[path, ..existing paths except path]`
  - Trim to `MaxCapacity` from the front
  - Return `new MruList { Paths = newPaths }`

### Task 1.2: Unit tests for `MruList`
File: `tests\D20Tek.Notepad.ViewModel.UnitTests\MruListTests.cs`
- [x] `Constructor_DefaultPaths_IsEmpty`
- [x] `Add_SinglePath_PathIsFirst`
- [x] `Add_DuplicatePath_MovesToFront`
- [x] `Add_DuplicatePath_DoesNotIncrementCount`
- [x] `Add_BeyondCapacity_OldestEntryDropped`
- [x] `Add_BeyondCapacity_CountRemainsAtMax`
- [x] `Add_NullPath_ThrowsArgumentNullException`
- [x] `Add_WhiteSpacePath_ThrowsArgumentException`
- [x] `Empty_HasNoPaths`

---

## Phase 2: ViewModel MRU Support

### Task 2.1: Add `RecentFiles` to `EditorSettings`
File: `src\D20Tek.Notepad.ViewModel\EditorSettings.cs`
- [x] Add `public IReadOnlyList<string> RecentFiles { get; init; } = [];`

### Task 2.2: Update `EditorSettings` unit tests
File: `tests\D20Tek.Notepad.ViewModel.UnitTests\EditorSettingsTests.cs`
- [x] Add `RecentFiles` to the Constructor test — assert default value is empty
- [x] Add `RecentFiles` to the existing With test — set a non-empty list and verify

### Task 2.3: Create `EditorViewModel.RecentFiles.cs`
File: `src\D20Tek.Notepad.ViewModel\EditorViewModel.RecentFiles.cs`
- [x] Declare `public sealed partial class EditorViewModel`
- [x] Add `public event Action<IReadOnlyList<string>>? RecentFilesChanged;`
- [x] Add `public IReadOnlyList<string> RecentFiles => _settings.RecentFiles;`
- [x] Add `internal void HookFilePathChanged()`:
  - Subscribe `FilePathChanged += OnFilePathChanged`
  - Called from the `EditorViewModel` primary constructor
- [x] Add `private void OnFilePathChanged(string? path)`:
  - Return early when `path` is null or whitespace
  - Build updated `MruList` from `_settings.RecentFiles` and call `.Add(path)`
  - Update `_settings = _settings with { RecentFiles = updated.Paths }`
  - Fire `RecentFilesChanged?.Invoke(_settings.RecentFiles)`

### Task 2.4: Wire `HookFilePathChanged()` in `EditorViewModel`
File: `src\D20Tek.Notepad.ViewModel\EditorViewModel.cs`
- [x] Call `HookFilePathChanged()` at the end of the primary constructor body

### Task 2.5: Unit tests for `EditorViewModel.RecentFiles`
File: `tests\D20Tek.Notepad.ViewModel.UnitTests\EditorViewModelTests.RecentFiles.cs`
- [x] `RecentFiles_WhenSettingsEmpty_ReturnsEmptyList`
- [x] `RecentFiles_WhenSettingsHasPaths_ReturnsPaths`
- [x] `SetFilePath_WithValidPath_AddsToRecentFiles`
- [x] `SetFilePath_WithSamePath_MovesPathToFront`
- [x] `SetFilePath_WithNullPath_DoesNotUpdateRecentFiles` (via `ClearFilePath()`)
- [x] `SetFilePath_FiresRecentFilesChangedEvent`
- [x] `SetFilePath_DoesNotFireRecentFilesChangedWhenPathNull`

---

## Phase 3: `OpenRecentCommand`

### Task 3.1: Create `OpenRecentCommand.cs`
File: `src\D20Tek.Notepad.Tui\Commands\OpenRecentCommand.cs`
- [x] Define `internal static class OpenRecentCommand`
- [x] Add `public const string CommandPrefix = "OpenRecent_";`
- [x] Add `public static void Execute(EditorViewModel viewModel, string filePath)`:
  - Guard `viewModel` not null
  - Return early if `UnsavedChangesHelper.PromptToSaveIfDirty()` returns `Cancel`
  - Load document via `DocumentFactory.Load(filePath)` inside try/catch
  - On success: call `viewModel.Session.ReplaceDocument(newDoc)`, `viewModel.Session.UndoStack.Clear()`,
    `viewModel.SetFilePath(Path.GetFullPath(filePath))`, `viewModel.ResetCleanVersion()`,
    `viewModel.Viewport.Reset()`, `viewModel.Refresh()`
  - On failure: show `MessageBox.ErrorQuery` with the exception message
- [x] Add `public static UiCommand Create(EditorViewModel viewModel, string filePath, int index)`:
  - Command name: `$"{CommandPrefix}{index}"`
  - No shortcut key
  - Action: `() => Execute(viewModel, filePath)`

---

## Phase 4: Menu Integration

### Task 4.1: Add `ActionEntry` to `MenuEntry.cs`
File: `src\D20Tek.Notepad.Tui\Menus\MenuEntry.cs`
- [x] Add `internal sealed record ActionEntry(string Label, Action Execute, Func<bool>? CanExecute = null) : MenuEntry`

### Task 4.2: Update `MenuBuilder` to handle `ActionEntry`
File: `src\D20Tek.Notepad.Tui\Menus\MenuBuilder.cs`
- [x] Add `ActionEntry` arm to the `BuildItem` switch expression (before the `_` throw arm):
  ```csharp
  ActionEntry a => a.CanExecute is null
      ? new MenuItem(a.Label, "", a.Execute)
      : new MenuItem(a.Label, "", a.Execute) { CanExecute = a.CanExecute },
  ```

### Task 4.3: Add MRU helper and submenu to `MenuDefinitions.cs`
File: `src\D20Tek.Notepad.Tui\Menus\MenuDefinitions.cs`
- [x] Add `private static IReadOnlyList<MenuEntry> BuildRecentFileItems(EditorViewModel viewModel)`:
  - When `viewModel.RecentFiles` is empty: return a single disabled placeholder:
    `[new ActionEntry("(Empty)", () => { }, CanExecute: () => false)]`
  - Otherwise: for index `i` (0-based), compute label `$"_{(i == 9 ? 0 : i + 1)} {Path.GetFileName(path)}"`,
    create `ActionEntry(label, () => OpenRecentCommand.Execute(viewModel, path))`
  - Return the built list
- [x] Add `Open _Recent` `SubMenuEntry` to the `_File` `TopLevelMenu` between `_Open...` and `_Save`:
  ```csharp
  new SubMenuEntry("Open _Recent", BuildRecentFileItems(viewModel)),
  ```

---

## Phase 5: Wiring in `Program.cs`

### Task 5.1: Subscribe to `RecentFilesChanged` and rebuild menu in `Program.cs`
File: `src\D20Tek.Notepad.Tui\Program.cs`
- [x] Capture the built `MenuBar` in a mutable local variable:
  `var menuBar = MenuBuilder.Build(viewModel, statusBarView);`
- [x] Change `top.Add(MenuBuilder.Build(...))` to `top.Add(menuBar);`
- [x] Add `RecentFilesChanged` handler that saves settings and rebuilds the menu bar:
  ```csharp
  viewModel.RecentFilesChanged += (_) =>
  {
      settingsService.Save(viewModel.Settings);
      top.Remove(menuBar);
      menuBar = MenuBuilder.Build(viewModel, statusBarView);
      top.Add(menuBar);
      top.SetNeedsDisplay();
  };
  ```

---

## File Changes Summary

### New Files
| File | Purpose |
|---|---|
| `src\D20Tek.Notepad.ViewModel\MruList.cs` | Immutable MRU list record with `Add()` and capacity cap |
| `src\D20Tek.Notepad.ViewModel\EditorViewModel.RecentFiles.cs` | Partial class: `RecentFiles` property, `RecentFilesChanged` event, `FilePathChanged` hook |
| `src\D20Tek.Notepad.Tui\Commands\OpenRecentCommand.cs` | Command to open a file from the MRU list |
| `tests\D20Tek.Notepad.ViewModel.UnitTests\MruListTests.cs` | 9 `MruList` unit tests |
| `tests\D20Tek.Notepad.ViewModel.UnitTests\EditorViewModelTests.RecentFiles.cs` | 7 ViewModel MRU tests |

### Modified Files
| File | Change |
|---|---|
| `src\D20Tek.Notepad.ViewModel\EditorSettings.cs` | Add `RecentFiles` property |
| `src\D20Tek.Notepad.ViewModel\EditorViewModel.cs` | Call `HookFilePathChanged()` in constructor |
| `src\D20Tek.Notepad.Tui\Menus\MenuEntry.cs` | Add `ActionEntry` record |
| `src\D20Tek.Notepad.Tui\Menus\MenuBuilder.cs` | Handle `ActionEntry` in `BuildItem` switch |
| `src\D20Tek.Notepad.Tui\Menus\MenuDefinitions.cs` | Add `Open _Recent` submenu + `BuildRecentFileItems()` helper |
| `src\D20Tek.Notepad.Tui\Program.cs` | Capture `menuBar`, subscribe `RecentFilesChanged`, rebuild on change |
| `tests\D20Tek.Notepad.ViewModel.UnitTests\EditorSettingsTests.cs` | Cover new `RecentFiles` property |

---

## Acceptance Criteria

- [x] `File > Recent` submenu appears in the menu bar
- [x] Submenu shows `(Empty)` placeholder (disabled, non-clickable) when no files have been opened
- [x] Opening a file via `File > Open` adds it to the top of the Recent list
- [x] Saving via `File > Save As` adds the new path to the top of the Recent list
- [x] Opening a file from CLI arguments adds it to the Recent list
- [x] Opening a file already in the list moves it to position 1 (no duplicates)
- [x] List is capped at 10 entries; the oldest is removed when a new one is added beyond capacity
- [x] Selecting a Recent file entry opens it (with unsaved-prompt behaviour identical to `File > Open`)
- [x] MRU list is persisted in `editor-settings.json` and restored on next launch
- [x] Menu is rebuilt and reflects the updated list immediately after any file open or save-as
- [x] All existing File, Edit, View, and Help menu items continue to work after the changes
