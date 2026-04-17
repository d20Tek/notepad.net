# Blazor WASM Client Feature Implementation

## Overview
Build a Blazor WebAssembly (WASM) client for the Notepad application, reusing `D20Tek.Notepad.Core` and `D20Tek.Notepad.ViewModel` to provide the same editing experience as the TUI in the browser. The Blazor client replaces Terminal.Gui rendering, dialogs, and platform services with browser-native equivalents.

---

## Architecture Design

### Project Structure
```
src/D20Tek.Notepad.Blazor/         ← Blazor WASM standalone app
├── wwwroot/
│   ├── index.html                 ← Host page
│   ├── css/
│   │   └── app.css                ← Editor and dialog styles
│   └── js/
│       └── notepad-interop.js     ← JS interop helpers (file, clipboard, keyboard)
├── Layout/
│   └── MainLayout.razor           ← App shell with menu bar and status bar
├── Pages/
│   └── Editor.razor               ← Main editor page
├── Components/
│   ├── EditorSurface.razor        ← Custom text rendering component (the "view")
│   ├── MenuBarComponent.razor     ← Top menu bar
│   ├── StatusBarComponent.razor   ← Bottom status bar
│   └── Dialogs/
│       ├── FindDialog.razor
│       ├── ReplaceDialog.razor
│       ├── GoToLineDialog.razor
│       ├── AboutDialog.razor
│       ├── UnsavedChangesDialog.razor
│       └── ConfirmDialog.razor
├── Services/
│   ├── BrowserClipboardService.cs ← IClipboardService via Clipboard API
│   └── BrowserFileService.cs      ← File open/save via File System Access API
├── Program.cs
└── D20Tek.Notepad.Blazor.csproj
```

### Layer Reuse Strategy
The TUI architecture has a clean separation:
- **Core** (`D20Tek.Notepad.Core`): Document model, editing, search, undo, storage — fully reusable.
- **ViewModel** (`D20Tek.Notepad.ViewModel`): `EditorViewModel`, `ViewLine`, `SelectionSegment`, `StatusDetails`, `IEditorRenderer`, `IClipboardService` — fully reusable.
- **TUI** (`D20Tek.Notepad.Tui`): Terminal.Gui-specific rendering, commands, dialogs — **replaced** by the Blazor project.

The Blazor client implements:
1. `IClipboardService` → `BrowserClipboardService` using the browser Clipboard API.
2. TUI Commands → Blazor command handlers calling the same `EditorViewModel` methods.
3. TUI Dialogs → Blazor modal dialog components.

`IEditorRenderer` is **not used** in Blazor — the `EditorSurface` component reads ViewModel state directly in declarative Razor markup, which is more idiomatic than the imperative adapter pattern designed for Terminal.Gui.

---

## Key Technical Challenges

### Challenge 1: File Open in a Sandboxed Browser

**Problem**: Blazor WASM runs in a browser sandbox with no direct file system access. The TUI uses `OpenDialog` to get a file path, then `DocumentFactory.Load(filePath)` which opens a `FileStream` internally.

**Solution**: Use the **File System Access API** (`showOpenFilePicker`) with JS interop fallback to `<input type="file">`.

**Approach**:
1. Call JS interop to trigger the browser file picker, which returns a `FileSystemFileHandle`.
2. Read the file as an `ArrayBuffer` via JS and pass the bytes back to C# as a `byte[]`.
3. Wrap the bytes in a `MemoryStream` and call `ITextStorageStrategy.Load(Stream)` directly (bypassing `DocumentFactory.Load(string filePath)` which expects a path).
4. Store the `FileSystemFileHandle` in JS for later "Save" operations (the handle retains write permission).
5. For browsers without File System Access API support, fall back to `<input type="file">` for open and Blob download for save.

**Key Code Path**:
```
Browser File Picker → JS interop → byte[] → MemoryStream → ITextStorageStrategy.Load(Stream) → DocumentData → DocumentFactory.Create(DocumentData) → IDocument
```

The `ITextStorageStrategy.Load(Stream)` and `ITextStorageStrategy.Save(Stream, IDocument)` interfaces are stream-based, making them directly usable without file paths.

### Challenge 2: File Save / Save As in a Sandboxed Browser

**Problem**: The TUI uses `SaveDialog` → file path → `DocumentFactory.Save(doc, filePath)` which writes to a `FileStream`.

**Solution**: Two strategies depending on browser API support:

**Strategy A — File System Access API** (Chrome, Edge):
1. For "Save" when a `FileSystemFileHandle` exists: call JS to get a writable stream on the handle, serialize document content to bytes in C#, pass bytes to JS, write and close.
2. For "Save As": call `showSaveFilePicker()` via JS interop to get a new handle, then same write flow.

**Strategy B — Fallback** (Firefox, Safari):
1. Serialize document to bytes in C#.
2. Create a Blob URL in JS and trigger a download with the filename.
3. "Save" and "Save As" both become downloads (no in-place overwrite possible).

**Key Code Path**:
```
EditorViewModel.Session.Document → ITextStorageStrategy.Save(MemoryStream, doc) → byte[] → JS interop → FileSystemFileHandle.createWritable() → write → close
```

### Challenge 3: Text Rendering and Editing

**Problem**: The TUI uses a custom `View` subclass (`EditorView`) with a `TerminalGuiRenderer` implementing `IEditorRenderer` to draw character-by-character via `Application.Driver`. The browser has no character-grid API.

**Solution**: Build a **custom Blazor component** (`EditorSurface.razor`) rather than using an existing HTML control. Reasons:

- **`<textarea>`**: Cannot render line numbers, custom selection highlighting, gutter, or word-wrap indicators. No control over caret rendering. Would bypass the ViewModel's rendering pipeline entirely.
- **`contenteditable` div**: Gives some rendering control but has wildly inconsistent behavior across browsers for cursor movement, selection, and input events. Fighting the browser's editing model would be harder than building from scratch.
- **Existing JS editors** (Monaco, CodeMirror): Would completely bypass the ViewModel layer, making Core and ViewModel unused. Defeats the purpose.
- **Custom component ✓**: Renders `VisibleLines` from `EditorViewModel` as positioned HTML `<div>` rows with `<span>` segments for normal text, selected text, and gutter. Keyboard input is captured directly on a `tabindex="0"` focusable div via a JS capture-phase `keydown` listener. Mouse events handled via Blazor event binding.

**Rendering Architecture**:
```
EditorViewModel.VisibleLines → EditorSurface.razor renders:
┌──────────────────────────────────────────┐
│ [gutter div] [text line div with spans]  │  ← per visible line
│ [gutter div] [text line div with spans]  │
│ ...                                      │
│ [blinking caret overlay div]             │
└──────────────────────────────────────────┘
     ↑ tabindex="0", focused div — receives all keyboard events
```

**Input Handling**:
- The editor surface `<div>` has `tabindex="0"` making it natively focusable with no hidden element needed.
- A JS `keydown` listener registered in **capture mode** on the editor div calls `preventDefault()` selectively for intercepted keys (all Ctrl combos, Tab, F3, etc.) before the browser acts on them. It then invokes a `DotNetObjectReference` callback to dispatch the key event to C#.
- C# processes every key: navigation (arrows, Home, End, PageUp, PageDown), modifier combos (Ctrl+Z/Y/A/C/V/X), editing keys (Enter, Backspace, Delete, Tab), and printable character keys via the `key` property.
- Paste (Ctrl+V) is handled asynchronously: detected in the JS callback, then `navigator.clipboard.readText()` is awaited in C# via `IJSRuntime.InvokeAsync`, and the text is passed to `EditorViewModel.Paste()`.
- Mouse events (`@onmousedown`, `@onmousemove`, `@onmouseup`, `@onwheel`) on the same div are translated to viewport-relative character grid positions and forwarded to `EditorViewModel` mouse handling methods.
- JS interop for font measurement using `getBoundingClientRect()` on a probe element to get exact character width/height for the current monospace font and zoom level.

**Rendering Refresh Cycle**:
- `EditorViewModel.ViewChanged` / `CaretMoved` / `SelectionChanged` events → call `StateHasChanged()` on the component.
- The component's `BuildRenderTree` reads `VisibleLines`, `CaretViewPosition`, and `SelectionViewRange` directly from the ViewModel to generate HTML.
- No need for `IEditorRenderer` frame-by-frame drawing — Blazor's diffing handles DOM updates efficiently. `BlazorEditorRenderer` can be a lightweight adapter or the component can read ViewModel state directly.

### Challenge 4: Keyboard Shortcut Handling

**Problem**: Browser intercepts many key combos (Ctrl+S, Ctrl+O, Ctrl+N, Ctrl+W, F5, Tab, etc.) before `@onkeydown` in Blazor fires, because Blazor's event handling runs after the browser's default action.

**Solution**: Register a JS `keydown` listener in **capture mode** (`{ capture: true }`) on the editor div. Capture-phase listeners fire before the browser acts on the event, allowing selective `preventDefault()`. The listener then invokes a `DotNetObjectReference` callback to dispatch key info into C#.

```javascript
// notepad-interop.js
export function registerKeyHandler(editorEl, dotNetRef) {
    editorEl.addEventListener('keydown', (e) => {
        if (shouldIntercept(e)) {
            e.preventDefault();
            dotNetRef.invokeMethodAsync('HandleKey',
                e.key, e.code, e.ctrlKey, e.shiftKey, e.altKey);
        }
    }, { capture: true });
}

function shouldIntercept(e) {
    if (e.ctrlKey) return true;   // intercept all Ctrl combos (Ctrl+S, Ctrl+O, etc.)
    if (e.key === 'Tab') return true;
    if (e.key === 'F3') return true;
    if (['ArrowUp','ArrowDown','ArrowLeft','ArrowRight',
         'Home','End','PageUp','PageDown'].includes(e.key)) return true;
    return false;
}
```

Blazer `@onkeydown` is **not** used for the editor — all key routing flows through the JS callback. This avoids the race condition between Blazor event handling and `preventDefault`, and centralises all key interception in one place.

---

## Task Breakdown

### Phase 1: Project Scaffolding

#### Task 1.1: Create Blazor WASM Project
- [ ] Create `D20Tek.Notepad.Blazor.csproj` targeting `net10.0` with `Microsoft.NET.Sdk.BlazorWebAssembly`
- [ ] Add project references to `D20Tek.Notepad.Core` and `D20Tek.Notepad.ViewModel`
- [ ] Add to the solution file
- [ ] Create `wwwroot/index.html` host page with monospace font loading
- [ ] Create `Program.cs` with service registration

#### Task 1.2: Create App Shell Layout
- [ ] Create `App.razor` with Router
- [ ] Create `MainLayout.razor` with menu bar, editor area, and status bar slots
- [ ] Create `wwwroot/css/app.css` with base editor styles (monospace font, dark theme, layout grid)

---

### Phase 2: JS Interop Foundation

#### Task 2.1: Create JS Interop Module
- [ ] Create `wwwroot/js/notepad-interop.js` with:
  - File picker functions (`openFilePicker`, `saveFilePicker`, `saveToHandle`)
  - Clipboard functions (`clipboardRead`, `clipboardWrite`)
  - Keyboard capture (`registerKeyHandler`, `preventDefault` for app shortcuts)
  - Font measurement helper (`measureCharWidth` for monospace grid calculations)
- [ ] Feature-detect File System Access API with `<input type="file">` fallback

#### Task 2.2: Implement BrowserFileService
- [ ] Create `BrowserFileService.cs` with JS interop calls
- [ ] `OpenFileAsync()` → triggers picker, returns `(byte[] content, string fileName)`
- [ ] `SaveFileAsync(byte[] content, string suggestedName)` → save via handle or download
- [ ] `SaveToExistingAsync(byte[] content)` → overwrite via stored handle
- [ ] `HasFileHandle` property → indicates if in-place save is possible
- [ ] Handle File System Access API unavailability gracefully (download fallback)

#### Task 2.3: Implement BrowserClipboardService
- [ ] Create `BrowserClipboardService.cs` implementing `IClipboardService`
- [ ] `SetText(string)` → JS `navigator.clipboard.writeText()`
- [ ] `GetText()` → JS `navigator.clipboard.readText()` (note: async in browser, may need adapter)
- [ ] `ContainsText` → best-effort check or always return true (like TUI)
- [ ] Handle clipboard permission prompts gracefully

---

### Phase 3: Editor Surface Component

#### Task 3.1: Create EditorSurface Component
- [ ] Create `EditorSurface.razor` component
- [ ] Render visible lines from `EditorViewModel.VisibleLines` as `<div class="line">` elements
- [ ] Render each line's text with `<span>` segments for normal and selected text
- [ ] Render gutter (line numbers) as `<div class="gutter-cell">` when enabled
- [ ] Render caret as a positioned `<div class="caret">` element with CSS blink animation
- [ ] Use monospace font with known character width for position calculations

#### Task 3.2: Wire Up ViewModel Events
- [ ] Subscribe to `ViewChanged`, `CaretMoved`, `SelectionChanged` → `StateHasChanged()`
- [ ] Subscribe to `WordWrapChanged`, `StatusBarChanged` → layout updates
- [ ] Initialize `EditorViewModel` with viewport dimensions on component `OnAfterRenderAsync`
- [ ] Handle browser resize events → update viewport dimensions via JS interop

#### Task 3.3: Implement Keyboard Input Handling
- [ ] Add `tabindex="0"` to the editor surface `<div>` and `@ref` for JS registration
- [ ] Register JS capture-phase `keydown` listener via `notepad-interop.js` on `OnAfterRenderAsync` with a `DotNetObjectReference` to the component
- [ ] Implement `[JSInvokable] HandleKey(string key, string code, bool ctrl, bool shift, bool alt)` method in the component to dispatch all key events
- [ ] Dispatch navigation keys (ArrowUp/Down/Left/Right, Home, End, PageUp, PageDown) → `EditorViewModel` navigation methods, with Shift held → selection extension
- [ ] Dispatch editing keys: Enter → `TypeEnter()`, Backspace → `Backspace()`, Delete → `DeleteForward()`, Tab → `TypeTab()`
- [ ] Dispatch modifier combos: Ctrl+Z → `Undo()`, Ctrl+Y → `Redo()`, Ctrl+A → `SelectAll()`, Ctrl+C → `Copy()`, Ctrl+X → `Cut()`
- [ ] Dispatch printable character keys (single `key` chars, no modifier) → `EditorViewModel.TypeCharacter()`
- [ ] Handle Ctrl+V (paste) asynchronously: call `navigator.clipboard.readText()` via `IJSRuntime.InvokeAsync`, then pass text to `EditorViewModel.Paste()`
- [ ] Ensure editor div receives focus on component mount and after dialog close

#### Task 3.4: Implement Mouse Input Handling
- [ ] Handle `@onmousedown` → calculate grid position → `EditorViewModel` caret placement
- [ ] Handle `@onmousemove` with button pressed → selection extension
- [ ] Handle `@onmouseup` → end selection
- [ ] Handle `@onwheel` → vertical and horizontal scrolling
- [ ] Handle double-click → word selection
- [ ] Convert pixel coordinates to character grid positions using known char width/height

#### Task 3.5: Implement Scrollbar Rendering
- [ ] Render vertical scrollbar track and thumb
- [ ] Render horizontal scrollbar (when word wrap is off)
- [ ] Handle scrollbar click and drag via mouse events
- [ ] Sync scrollbar state with `EditorViewModel.Viewport`

---

### Phase 4: Menu Bar and Commands

#### Task 4.1: Create Menu Bar Component
- [ ] Create `MenuBarComponent.razor` with top-level menu items (File, Edit, View, Help)
- [ ] Implement dropdown menu rendering with CSS
- [ ] Wire menu items to command handlers
- [ ] Show keyboard shortcut hints in menu items
- [ ] Support checkable menu items (Word Wrap, Status Bar, Line Numbers, Overwrite Mode)
- [ ] Support disabled menu items with `CanExecute` checks

#### Task 4.2: Implement File Commands
- [ ] New → `EditorViewModel.Session.ReplaceDocument(factory.Empty)` with unsaved changes check
- [ ] Open → `BrowserFileService.OpenFileAsync()` → stream-based load → apply to ViewModel
- [ ] Save → serialize to stream → `BrowserFileService.SaveToExistingAsync()` or Save As fallback
- [ ] Save As → serialize to stream → `BrowserFileService.SaveFileAsync()`
- [ ] Encoding submenu → `EditorViewModel.ChangeEncoding()`

#### Task 4.3: Implement Edit Commands
- [ ] Undo / Redo → `EditorViewModel.Undo()` / `EditorViewModel.Redo()`
- [ ] Cut / Copy / Paste → `EditorViewModel` clipboard methods + `BrowserClipboardService`
- [ ] Find / Find Next / Find Previous → open `FindDialog`
- [ ] Replace → open `ReplaceDialog`
- [ ] Go to Line → open `GoToLineDialog`
- [ ] Select All → `EditorViewModel.SelectAll()`

#### Task 4.4: Implement View Commands
- [ ] Toggle Status Bar → `EditorViewModel.ToggleStatusBar()`
- [ ] Toggle Word Wrap → `EditorViewModel.ToggleWordWrap()`
- [ ] Toggle Line Numbers → `EditorViewModel.ToggleLineNumbers()`
- [ ] Toggle Overwrite Mode → `EditorViewModel.ToggleOverwriteMode()`
- [ ] Zoom In / Zoom Out → adjust CSS `font-size` and recalculate viewport

---

### Phase 5: Dialog Components

#### Task 5.1: Create Dialog Infrastructure
- [ ] Create base modal dialog CSS (overlay, centered panel, focus trap)
- [ ] Create `ConfirmDialog.razor` for simple Yes/No/Cancel prompts
- [ ] Create `UnsavedChangesDialog.razor` (Save / Don't Save / Cancel)
- [ ] Wire dialogs to return results via `TaskCompletionSource<T>` or callback pattern

#### Task 5.2: Create Find Dialog
- [ ] Create `FindDialog.razor` matching TUI FindDialog functionality
- [ ] Search term text input, Case Sensitive checkbox, Wrap Around checkbox
- [ ] Find Next / Find Previous / Close buttons
- [ ] Return `FindAction` and `SearchOptions` to caller
- [ ] Keep dialog open for repeated searches (non-modal or side panel approach)

#### Task 5.3: Create Replace Dialog
- [ ] Create `ReplaceDialog.razor` matching TUI ReplaceDialog functionality
- [ ] Search term + replacement text inputs, options checkboxes
- [ ] Find Next / Replace / Replace All / Close buttons
- [ ] Return `ReplaceAction` with search/replace terms

#### Task 5.4: Create Go To Line Dialog
- [ ] Create `GoToLineDialog.razor` matching TUI GoToLineDialog functionality
- [ ] Line number input with validation (1 to total lines)
- [ ] Go To / Cancel buttons

#### Task 5.5: Create About Dialog
- [ ] Create `AboutDialog.razor` matching TUI AboutDialog content
- [ ] App name, version, description, copyright, repository link
- [ ] OK button to close

---

### Phase 6: Status Bar Component

#### Task 6.1: Create Status Bar Component
- [ ] Create `StatusBarComponent.razor`
- [ ] Display: `Ln X, Col Y` | total lines, total characters | encoding | line endings | overwrite mode
- [ ] Subscribe to `EditorViewModel.StatusChanged` for real-time updates
- [ ] Support transient messages (like zoom hints)
- [ ] Toggle visibility based on `EditorViewModel.IsStatusBarEnabled`

---

### Phase 7: Styling and Polish

#### Task 7.1: Editor Theme and Styling
- [ ] Define CSS custom properties for theme colors (background, foreground, selection, gutter)
- [ ] Style editor surface with dark theme matching TUI appearance
- [ ] Style caret with CSS animation (`@keyframes blink`)
- [ ] Style selection highlighting with background color
- [ ] Style gutter with distinct background and text color
- [ ] Ensure monospace font consistency (`Cascadia Code`, `Consolas`, `monospace` fallback)

#### Task 7.2: Responsive Layout
- [ ] Editor surface fills available space (flexbox/grid layout)
- [ ] Handle window resize → recalculate viewport dimensions
- [ ] Handle mobile viewport considerations (optional, lower priority)

#### Task 7.3: Loading and Error States
- [ ] Show loading indicator for large file reads
- [ ] Display error messages for failed file operations (toast or inline)
- [ ] Handle browser permission denials gracefully (clipboard, file access)

---

### Phase 8: Testing

#### Task 8.1: Unit Tests for Browser Services
- [ ] Create `D20Tek.Notepad.Blazor.UnitTests` project
- [ ] Test `BrowserClipboardService` with mocked JS interop
- [ ] Test `BrowserFileService` with mocked JS interop
- [ ] Test file content round-trip: open → edit → save → verify bytes

#### Task 8.2: Component Tests
- [ ] Test `EditorSurface` renders visible lines correctly
- [ ] Test `EditorSurface` keyboard input flows to ViewModel
- [ ] Test dialog components return correct results
- [ ] Test menu bar command wiring
- [ ] Test status bar updates on ViewModel changes

---

### Phase 9: Integration and Deployment

#### Task 9.1: End-to-End Verification
- [ ] Verify file open/save cycle works in Chrome (File System Access API)
- [ ] Verify file open/save cycle works in Firefox/Safari (fallback)
- [ ] Verify clipboard operations across browsers
- [ ] Verify all keyboard shortcuts work without browser interception
- [ ] Verify undo/redo, find/replace, go-to-line all function correctly
- [ ] Verify word wrap, line numbers, status bar toggles work
- [ ] Verify selection with keyboard and mouse

#### Task 9.2: Build and Publish Configuration
- [ ] Configure AOT compilation or trimming settings for WASM size optimization
- [ ] Add `<ServiceWorkerAssetsManifest>` for optional PWA support
- [ ] Configure static file hosting (GitHub Pages, Azure Static Web Apps, or similar)

---

## Feature Parity Matrix (TUI → Blazor)

| TUI Feature              | Blazor Equivalent                               | Status |
|--------------------------|--------------------------------------------------|--------|
| Terminal.Gui `View`      | `EditorSurface.razor` custom component           | [ ]    |
| `TerminalGuiRenderer`    | Direct ViewModel state → HTML rendering          | [ ]    |
| `TerminalGuiClipboard`   | `BrowserClipboardService` via Clipboard API      | [ ]    |
| `OpenDialog`             | File System Access API / `<input type="file">`   | [ ]    |
| `SaveDialog`             | File System Access API / Blob download            | [ ]    |
| `FindDialog`             | `FindDialog.razor` modal                         | [ ]    |
| `ReplaceDialog`          | `ReplaceDialog.razor` modal                      | [ ]    |
| `GoToLineDialog`         | `GoToLineDialog.razor` modal                     | [ ]    |
| `AboutDialog`            | `AboutDialog.razor` modal                        | [ ]    |
| `MessageBox` prompts     | `ConfirmDialog.razor` / `UnsavedChangesDialog`   | [ ]    |
| `MenuBar`                | `MenuBarComponent.razor`                         | [ ]    |
| `StatusBar`              | `StatusBarComponent.razor`                       | [ ]    |
| Keyboard shortcuts       | JS interop `keydown` handler + `preventDefault`  | [ ]    |
| Mouse editing            | Blazor mouse events + pixel→grid calculation     | [ ]    |
| Scrollbars               | Custom HTML scrollbar elements                   | [ ]    |
| Zoom In/Out              | CSS `font-size` adjustment                       | [ ]    |
| Color scheme             | CSS custom properties (dark theme)               | [ ]    |
| Settings persistence     | `localStorage` via JS interop                    | [ ]    |
| Command-line file open   | N/A (browser has no CLI; could use URL params)   | [ ]    |
| MRU / Recent Files       | `localStorage`-backed list                       | [ ]    |
| Large file loading       | Progressive stream read with progress indicator  | [ ]    |

---

## Technical Notes

### IEditorRenderer in Blazor
The TUI implements `IEditorRenderer` with imperative `Driver.Move()` / `Driver.AddStr()` calls. In Blazor, rendering is declarative (Razor markup). Two approaches:

**Option A — Skip IEditorRenderer**: The `EditorSurface` component reads `VisibleLines`, `CaretViewPosition`, and `SelectionViewRange` directly from the ViewModel in its `BuildRenderTree`. The ViewModel's `RenderFrame()` method (which calls `IEditorRenderer`) is not used. This is simpler and more idiomatic for Blazor.

**Option B — Adapter**: Implement `BlazorEditorRenderer` that collects render commands into a list during `RenderFrame()`, then the component reads that list. Adds indirection without benefit.

**Recommendation**: Option A. The ViewModel already exposes all the state needed for rendering. The `IEditorRenderer` abstraction was designed for imperative rendering frameworks and is not necessary in a declarative UI framework.

### Async Clipboard in Blazor
The browser Clipboard API is async (`navigator.clipboard.readText()` returns a Promise), but `IClipboardService.GetText()` is synchronous. Options:
1. Change `IClipboardService` to async (breaking change to ViewModel).
2. Use a sync-over-async adapter with `JSRuntime.InvokeAsync` and `.Result` (risks deadlock in WASM single-thread).
3. Pre-read clipboard into a cache on focus/keydown events, serve from cache synchronously.
4. Use `IJSInProcessRuntime` which supports synchronous JS calls in WASM.

**Chosen approach**: Because the JS `keydown` capture handler already detects Ctrl+V before dispatching to C#, paste is handled as a **dedicated async code path** in `HandleKey` rather than flowing through `IClipboardService.GetText()` at all:

```csharp
// In HandleKey — paste bypasses IClipboardService entirely
if (ctrl && key == "v")
{
    var text = await _jsRuntime.InvokeAsync<string>("navigator.clipboard.readText");
    _viewModel.Paste(text);
    return;
}
```

Cut and Copy (`IClipboardService.SetText`) remain synchronous because `navigator.clipboard.writeText()` is fire-and-forget — the WASM thread does not need to await the write to complete before continuing. `BrowserClipboardService.SetText()` calls `IJSInProcessRuntime.InvokeVoid("navigator.clipboard.writeText", text)`, keeping the interface synchronous. `ContainsText` always returns `true` (same reasoning as `TerminalGuiClipboardService`).

### File Name Tracking
The TUI tracks `CurrentFilePath` as a full file path. In Blazor:
- Store the file name (from `File.name`) for display in the title bar.
- Store the `FileSystemFileHandle` in JS for save-back capability.
- `CurrentFilePath` in the ViewModel receives just the file name (not a full path, since browser sandboxing hides paths).
