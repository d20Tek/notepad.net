# Help Menu Feature Tasks

## Overview

Add a **Help** top-level menu to the menu bar with two items:

| Menu Item | Shortcut | Notes |
|-----------|----------|-------|
| Getting Started | F1 | Opens the project documentation page in the default browser |
| About Notepad.Tui | — | Shows a modal About dialog with app name, version, description, and copyright |

---

## Architecture

The feature follows the existing patterns in the codebase:

- **Commands** – static classes in `src/D20Tek.Notepad.Tui/Commands/` with a `CommandName` constant, a static `Execute()` method, and a static `Create()` factory returning a `UiCommand`.
- **Dialogs** – sealed classes in `src/D20Tek.Notepad.Tui/Dialogs/` with a `Show()` method and a static `Create()` factory.
- **Menu wiring** – `MenuDefinitions.cs` declares the menu structure; `MenuCommands.cs` registers the commands.

Neither command touches `EditorViewModel` — they are pure UI concerns that need no ViewModel changes.

---

## Tasks

### Task 1 — Create `HelpGettingStartedCommand`

**File:** `src/D20Tek.Notepad.Tui/Commands/HelpGettingStartedCommand.cs`

- Define `CommandName = "HelpGettingStarted"`.
- `Execute()` calls `Process.Start(new ProcessStartInfo(Url) { UseShellExecute = true })` to open the URL in the default system browser. The URL is the project's public documentation page: `https://github.com/d20Tek/notepad.net`.
- `Create()` returns `new UiCommand(CommandName, Execute, Key.F1)`.
- Wrap `Process.Start` in a try/catch so an unresolvable browser does not crash the editor; surface the error via `MessageBox.ErrorQuery`.

```
internal static class HelpGettingStartedCommand
{
    public const string CommandName = "HelpGettingStarted";
    private const string Url = "https://github.com/d20Tek/notepad.net";

    public static void Execute() { ... }
    public static UiCommand Create() => new(CommandName, Execute, Key.F1);
}
```

---

### Task 2 — Create `AboutDialog`

**File:** `src/D20Tek.Notepad.Tui/Dialogs/AboutDialog.cs`

Build a Terminal.Gui `Dialog` that displays:

| Field | Source |
|-------|--------|
| App name | `"Notepad.Tui"` (hardcoded constant) |
| Version | `Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion` |
| Description | Short hardcoded string (matches the `.csproj` `<Description>`) |
| Copyright | `"Copyright © d20Tek. All rights reserved."` |
| Repository | `https://github.com/d20Tek/notepad.net` |

Layout guidelines:
- Dialog width: `60`, height: `12`.
- Stack labels vertically starting at `Y = 1`, left-padded `X = 2`.
- Single **OK** button centered at `Y = 9` that calls `Application.RequestStop()`.
- Use `Label` views for each field; no text fields needed (read-only).

```
internal sealed class AboutDialog
{
    public void Show() { ... }
    public static AboutDialog Create() => new();
}
```

---

### Task 3 — Create `HelpAboutCommand`

**File:** `src/D20Tek.Notepad.Tui/Commands/HelpAboutCommand.cs`

- Define `CommandName = "HelpAbout"`.
- `Execute()` calls `AboutDialog.Create().Show()`.
- `Create()` returns `new UiCommand(CommandName, Execute)` (no shortcut key).

```
internal static class HelpAboutCommand
{
    public const string CommandName = "HelpAbout";

    public static void Execute() { ... }
    public static UiCommand Create() => new(CommandName, Execute);
}
```

---

### Task 4 — Register Commands in `MenuCommands.cs`

**File:** `src/D20Tek.Notepad.Tui/Menus/MenuCommands.cs`

Add a `// Help commands` section and register both new commands:

```csharp
// Help commands
commands.Register(HelpGettingStartedCommand.Create());
commands.Register(HelpAboutCommand.Create());
```

Note: `UiCommand.Create()` for these commands does **not** take an `EditorViewModel` parameter.
`CommandRegistry.Register` accepts any `UiCommand`, so the call is unchanged.

---

### Task 5 — Add Help Top-Level Menu in `MenuDefinitions.cs`

**File:** `src/D20Tek.Notepad.Tui/Menus/MenuDefinitions.cs`

Append a new `TopLevelMenu` entry for **_Help** at the end of the array returned by `Get()`:

```csharp
new TopLevelMenu("_Help",
    new CommandEntry("_Getting Started", HelpGettingStartedCommand.CommandName),
    new CommandEntry("_About Notepad.Tui", HelpAboutCommand.CommandName))
```

---

### Task 6 — Update `CommandRegistry` to Support Parameterless Commands (if needed)

**File:** `src/D20Tek.Notepad.Tui/Commands/CommandRegistry.cs`

Verify that `CommandRegistry.Register(UiCommand)` works with commands whose `Create()` factory takes no `EditorViewModel`. If `Register` already accepts a plain `UiCommand`, no change is needed. If there is an overload mismatch, add an appropriate overload.

---

### Task 7 — Manual Smoke Test Checklist

Because `AboutDialog` and `HelpGettingStartedCommand` both depend on the Terminal.Gui runtime (`Application.Run`) and the OS shell (`Process.Start`), they cannot be unit-tested without the full TUI stack. A manual smoke test is required:

| Test | Expected Result |
|------|----------------|
| Launch app, open **Help** menu | Menu shows "Getting Started" (F1) and "About Notepad.Tui" |
| Click **Getting Started** or press F1 | Default browser opens `https://github.com/d20Tek/notepad.net` |
| Click **About Notepad.Tui** | Modal dialog appears with correct name, version, description, and copyright |
| Press **OK** in About dialog | Dialog closes; editor resumes normally |
| Press F1 while editing | Browser opens (same as menu item) |

---

### Task 8 — Update Feature Roadmap

**File:** `plans/features.md`

Move `Help>About dialog` and `Help>Getting Started` from the **Pending** table to the **Implemented** table and mark them with their respective shortcuts.

---

## File Summary

| File | Action |
|------|--------|
| `src/D20Tek.Notepad.Tui/Commands/HelpGettingStartedCommand.cs` | **Create** |
| `src/D20Tek.Notepad.Tui/Commands/HelpAboutCommand.cs` | **Create** |
| `src/D20Tek.Notepad.Tui/Dialogs/AboutDialog.cs` | **Create** |
| `src/D20Tek.Notepad.Tui/Menus/MenuCommands.cs` | **Modify** – register two new commands |
| `src/D20Tek.Notepad.Tui/Menus/MenuDefinitions.cs` | **Modify** – add Help top-level menu |
| `src/D20Tek.Notepad.Tui/Commands/CommandRegistry.cs` | **Verify / Modify** if parameterless `Create()` is incompatible |
| `plans/features.md` | **Modify** – move items from Pending to Implemented |

---

## Implementation Notes

- **No ViewModel changes** – the Help commands are pure UI and carry no document state.
- **Assembly version** – resolved at runtime via reflection so it stays in sync with the `.csproj` version automatically.
- **Browser launch** – `UseShellExecute = true` is required on all platforms; `Process.Start` without it silently fails on Linux/macOS.
- **Dialog style** – follow the same `new Dialog(title, width, height)` constructor style used in `GoToLineDialog` and `FindDialog`; do not use `Pos.Center()` for the dialog itself (Terminal.Gui centers it automatically).
