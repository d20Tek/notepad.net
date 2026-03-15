# Zoom Feature Tasks

## Overview

Implement a **Zoom** feature (View > Zoom In / Zoom Out, `Ctrl+=` / `Ctrl+-`) that educates the
user about terminal-level font zoom rather than attempting application-level font scaling, which
is not possible from within a Terminal.Gui process.

**Behaviour:**
- If the status bar is **visible**: display a self-dismissing transient message for 4 seconds, then
  restore normal status bar content. The editor remains fully interactive during the hint.
- If the status bar is **hidden**: show a `MessageBox.Query` with the same hint text.

No `EditorViewModel` state changes. No new `EditorSettings` fields. This is a pure TUI-layer feature.

---

## User-Visible Result

```
 File  Edit  View  Help
 ??????????????????????????????????????????????????????????????????????????????
 <editor content>
 ??????????????????????????????????????????????????????????????????????????????
 Zoom: use Ctrl+Scroll or Ctrl+Plus/Minus in your terminal emulator (e.g. Windows Terminal)
 ^^ transient message replaces normal "Ln/Col | UTF-8 | CRLF" for 4 seconds ^^
```

---

## Architecture

```
Program.cs
  ?? MenuBuilder.Build(viewModel, statusBarView)        ? new parameter
       ?? MenuCommands.Get(viewModel, statusBarView)    ? new parameter
            ?? ZoomCommands.CreateZoomIn(viewModel, statusBarView)
            ?? ZoomCommands.CreateZoomOut(viewModel, statusBarView)
                 ?? ZoomCommands.ShowHint(viewModel, statusBarView)
                      ?? viewModel.IsStatusBarEnabled == true
                      ?    ?? statusBarView.ShowTransientMessage(hint, TimeSpan.FromSeconds(4))
                      ?? viewModel.IsStatusBarEnabled == false
                           ?? MessageBox.Query("Zoom", hintText, "OK")
```

---

## Tasks

### Task 1 � Add `ShowTransientMessage` to `StatusBarView`

**File:** `src/D20Tek.Notepad.Tui/StatusBarView.cs`

Add a `string? _transientMessage` field. Implement `ShowTransientMessage(string message, TimeSpan duration)`:
- Set `_transientMessage` and call `SetNeedsDisplay()`.
- Schedule an `Application.MainLoop.AddTimeout` for `duration` that clears `_transientMessage`
  and calls `SetNeedsDisplay()` � returning `false` so the timer does not repeat.
- Guard with `if (_disposed) return` at the top.

Update `Redraw()` to branch at the top: when `_transientMessage is not null`, render the message
left-aligned (with a 1-space margin, clipped to `bounds.Width - 2`) and `return` early, bypassing
the normal position/statistics/encoding sections. This means the transient message takes over the
full status bar row for its duration.

```csharp
// new field
private string? _transientMessage;

// new public method
public void ShowTransientMessage(string message, TimeSpan duration) { ... }

// updated Redraw � new branch at top, before existing left/center/right logic
if (_transientMessage is not null)
{
    Move(1, 0);
    Driver.AddStr(/* clipped _transientMessage */);
    return;
}
```

---

### Task 2 � Create `ZoomCommands.cs`

**File:** `src/D20Tek.Notepad.Tui/Commands/ZoomCommands.cs` *(new)*

Follow the `FindCommands.cs` / `ClipboardCommands.cs` pattern of grouping related commands in one
file. Define two command names and two `Create` factories that share a single `ShowHint` method.

```csharp
internal static class ZoomCommands
{
    public const string ZoomInCommandName  = "ZoomIn";
    public const string ZoomOutCommandName = "ZoomOut";

    private const string HintMessage =
        "Zoom: use Ctrl+Scroll or Ctrl+Plus/Minus in your terminal (e.g. Windows Terminal)";
    private const string MessageBoxText =
        "Text zoom is controlled by your terminal emulator.\n\n" +
        "Use Ctrl+Scroll or Ctrl+Plus/Minus\nin Windows Terminal or your terminal app.";

    public static void ShowHint(EditorViewModel viewModel, StatusBarView statusBarView) { ... }

    public static UiCommand CreateZoomIn(EditorViewModel viewModel, StatusBarView statusBarView) =>
        new(ZoomInCommandName, () => ShowHint(viewModel, statusBarView), Key.CtrlMask | Key.EqualsSign);

    public static UiCommand CreateZoomOut(EditorViewModel viewModel, StatusBarView statusBarView) =>
        new(ZoomOutCommandName, () => ShowHint(viewModel, statusBarView), Key.CtrlMask | Key.Minus);
}
```

`ShowHint` checks `viewModel.IsStatusBarEnabled`:
- **true** ? `statusBarView.ShowTransientMessage(HintMessage, TimeSpan.FromSeconds(4))`
- **false** ? `MessageBox.Query("Zoom", MessageBoxText, "OK")`

> **Note:** Verify the exact `Key` values for `Ctrl+=` and `Ctrl+-` against the Terminal.Gui
> version in use. If `Key.EqualsSign` is unavailable, use the raw char cast `(Key)'='` with
> `Key.CtrlMask`. Adjust in `CreateZoomIn`/`CreateZoomOut` as needed without changing `ShowHint`.

---

### Task 3 � Extend `MenuCommands.Get()` to Accept `StatusBarView`

**File:** `src/D20Tek.Notepad.Tui/Menus/MenuCommands.cs`

Change the signature from `Get(EditorViewModel viewModel)` to
`Get(EditorViewModel viewModel, StatusBarView statusBarView)`.

Add a `// Zoom commands` section and register both new commands:

```csharp
// Zoom commands
commands.Register(ZoomCommands.CreateZoomIn(viewModel, statusBarView));
commands.Register(ZoomCommands.CreateZoomOut(viewModel, statusBarView));
```

---

### Task 4 � Extend `MenuBuilder.Build()` to Accept `StatusBarView`

**File:** `src/D20Tek.Notepad.Tui/Menus/MenuBuilder.cs`

Change the public signature from `Build(EditorViewModel viewModel)` to
`Build(EditorViewModel viewModel, StatusBarView statusBarView)`.

Update the internal call:

```csharp
// before
var commands = MenuCommands.Get(viewModel);
// after
var commands = MenuCommands.Get(viewModel, statusBarView);
```

---

### Task 5 � Add Zoom Items to the View Menu in `MenuDefinitions.cs`

**File:** `src/D20Tek.Notepad.Tui/Menus/MenuDefinitions.cs`

Append a separator and two new `CommandEntry` items to the existing `_View` `TopLevelMenu`:

```csharp
new TopLevelMenu("_View",
    // ... existing Status Bar, Word Wrap, Line Numbers entries ...
    MenuEntry.Separator,
    new CommandEntry("Zoom _In",  ZoomCommands.ZoomInCommandName),
    new CommandEntry("Zoom _Out", ZoomCommands.ZoomOutCommandName))
```

---

### Task 6 � Update `Program.cs` to Pass `statusBarView` to `MenuBuilder`

**File:** `src/D20Tek.Notepad.Tui/Program.cs`

`statusBarView` is already constructed before the menu is built, so only the call site changes:

```csharp
// before
top.Add(MenuBuilder.Build(viewModel));
// after
top.Add(MenuBuilder.Build(viewModel, statusBarView));
```

---

### Task 7 � Add `Ctrl+ScrollWheel` Binding in `EditorView`

**Files:** `src/D20Tek.Notepad.Tui/EditorView.cs` *(modify)* � `src/D20Tek.Notepad.Tui/Program.cs` *(modify)*

Terminal.Gui surfaces the Ctrl modifier on mouse events via `MouseFlags.ButtonCtrl`. Ctrl+ScrollWheel
must be intercepted **before** `MouseBindings.TryExecute` is called, because `TryExecute` checks
for plain `WheeledUp`/`WheeledDown` first and would consume the event without the Ctrl guard.

**Step 1 � Add a `ZoomRequested` event to `EditorView`**

Keep `EditorView` decoupled from `StatusBarView` by exposing a plain event that the composition
root (`Program.cs`) wires to the zoom hint logic:

```csharp
public event Action? ZoomRequested;
```

**Step 2 � Detect Ctrl+Scroll in `EditorView.MouseEvent`**

Add a Ctrl+scroll guard at the top of `MouseEvent`, before the `IsInScrollBarArea` and
`MouseBindings.TryExecute` checks:

```csharp
public override bool MouseEvent(MouseEvent me)
{
    var flags = me.Flags;

    // Ctrl+ScrollWheel ? zoom hint (intercept before plain scroll in MouseBindings)
    if (flags.HasFlag(MouseFlags.ButtonCtrl) &&
        (flags.HasFlag(MouseFlags.WheeledUp) || flags.HasFlag(MouseFlags.WheeledDown)))
    {
        ZoomRequested?.Invoke();
        return true;
    }

    // ... existing bounds check, scroll-bar, gutter, and MouseBindings.TryExecute calls
}
```

**Step 3 � Wire `ZoomRequested` in `Program.cs`**

`editorView` and `statusBarView` are both available at composition time. Subscribe after both
are created, in the same block as the other `viewModel` event subscriptions:

```csharp
editorView.ZoomRequested += () => ZoomCommands.ShowHint(viewModel, statusBarView);
```

> **Note:** `MouseFlags.ButtonCtrl` is the Terminal.Gui flag for a Ctrl-held mouse event.
> Verify against the `MouseFlags` enum in the Terminal.Gui version referenced by the project.
> If the flag name differs, adjust only the `HasFlag` call � the event wiring is unaffected.

---

### Task 8 � Verify Build Succeeds

Run a full solution build to confirm no compile errors from the signature changes in
`MenuBuilder` and `MenuCommands`.

---

### Task 9 � Manual Smoke Test Checklist

| Test | Expected |
|------|----------|
| View > Zoom In (status bar visible) | Status bar shows hint for ~4 s, then restores `Ln/Col` content |
| View > Zoom Out (status bar visible) | Same hint behaviour as Zoom In |
| `Ctrl+=` keyboard shortcut (status bar visible) | Same as menu item |
| `Ctrl+-` keyboard shortcut (status bar visible) | Same as menu item |
| `Ctrl+ScrollWheel Up` (status bar visible) | Same hint as Zoom In menu item |
| `Ctrl+ScrollWheel Down` (status bar visible) | Same hint as Zoom Out menu item |
| View > Zoom In (status bar **hidden**) | `MessageBox` appears with hint; dismisses cleanly |
| View > Zoom Out (status bar **hidden**) | Same `MessageBox` behaviour |
| `Ctrl+ScrollWheel` (status bar **hidden**) | `MessageBox` appears with hint; dismisses cleanly |
| Plain scroll (no Ctrl) unchanged | Editor scrolls normally; no hint shown |
| Press Zoom while hint already showing | Timer resets; hint remains visible for another 4 s |
| Resize terminal during hint | Hint stays visible, clips correctly to new width |

---

### Task 10 � Update Feature Roadmap

**File:** `plans/features.md`

Move **Zoom** from the **Pending** table to the **Implemented** table:

```markdown
| Zoom | View menu / Ctrl+=, Ctrl+- | Educates user to use terminal zoom shortcut |
```

Strike through the Pending row and update the Implementation Priority list.

---

## File Summary

| File | Action |
|------|--------|
| `src/D20Tek.Notepad.Tui/StatusBarView.cs` | **Modify** � add `_transientMessage` field, `ShowTransientMessage()`, branch in `Redraw()` |
| `src/D20Tek.Notepad.Tui/Commands/ZoomCommands.cs` | **Create** � `ZoomInCommandName`, `ZoomOutCommandName`, `ShowHint()`, two `Create` factories |
| `src/D20Tek.Notepad.Tui/Menus/MenuCommands.cs` | **Modify** � add `StatusBarView` parameter, register zoom commands |
| `src/D20Tek.Notepad.Tui/Menus/MenuBuilder.cs` | **Modify** � add `StatusBarView` parameter, pass to `MenuCommands.Get()` |
| `src/D20Tek.Notepad.Tui/Menus/MenuDefinitions.cs` | **Modify** � add separator + Zoom In / Zoom Out to View menu |
| `src/D20Tek.Notepad.Tui/EditorView.cs` | **Modify** � add `ZoomRequested` event; detect `Ctrl+ScrollWheel` before `MouseBindings.TryExecute` |
| `src/D20Tek.Notepad.Tui/Program.cs` | **Modify** � pass `statusBarView` to `MenuBuilder.Build()`; wire `editorView.ZoomRequested` |
| `plans/features.md` | **Modify** � move Zoom to Implemented |

---

## Implementation Notes

- **No ViewModel changes** � `IsStatusBarEnabled` is read at execution time via the existing
  `viewModel.IsStatusBarEnabled` property; no new events or settings fields are needed.
- **Timer safety** � `ShowTransientMessage` guards with `if (_disposed) return` and the timeout
  callback checks `!_disposed` before calling `SetNeedsDisplay()`, matching the pattern already
  used in `OnStatusChanged`.
- **Concurrent hints** � If `ShowTransientMessage` is called while a hint is already showing, the
  new message replaces the old one immediately and a fresh timer starts. The old timer will fire
  later but find `_transientMessage` already replaced or null and simply call `SetNeedsDisplay()`
  harmlessly.
- **Key code verification** � `Ctrl+=` terminal delivery varies by OS and terminal emulator. If
  the shortcut does not fire, it can be removed from `UiCommand` without affecting the menu item
  or the `ShowHint` logic.
- **Ctrl+Scroll decoupling** � `EditorView` fires a plain `Action? ZoomRequested` event rather
  than taking a direct dependency on `StatusBarView`. This keeps the mouse-input layer ignorant of
  status bar state and lets `Program.cs` own the wiring, consistent with how `StatusBarChanged`
  and other cross-view events are already composed there.
- **Ctrl+Scroll intercept order** � the Ctrl+scroll guard must appear in `EditorView.MouseEvent`
  **before** the call to `MouseBindings.TryExecute`. `TryExecute` checks `WheeledUp`/`WheeledDown`
  without a Ctrl guard, so it would silently consume the event and scroll the document instead of
  showing the zoom hint.
