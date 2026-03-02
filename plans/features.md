# Notepad.Tui Feature Roadmap

## Implemented Features

| Feature | Shortcut | Notes |
|---------|----------|-------|
| Text input (typing) | - | Character input |
| Backspace | Backspace | Delete char before caret |
| Delete | Delete | Delete char after caret |
| Enter | Enter | Insert new line |
| Tab | Tab | Insert tab/spaces |
| Undo | Ctrl+Z | With typing group support |
| Redo | Ctrl+Y, Ctrl+Shift+Z | |
| Select All | Ctrl+A | |
| File New | Ctrl+N | With unsaved prompt |
| File Open | Ctrl+O | With unsaved prompt |
| File Save | Ctrl+S | |
| File Save As | Ctrl+Shift+S | |
| Word Wrap | View menu | Toggle |
| Dirty tracking | - | Version-based, undo-aware |
| Title bar | - | Shows filename + dirty indicator |
| **Cut/Copy/Paste** |
| Cut | Ctrl+X | Remove selection, copy to clipboard |
| Copy | Ctrl+C | Copy selection to clipboard |
| Paste | Ctrl+V | Insert clipboard content |
| **Find/Replace** |
| Find | Ctrl+F | Search dialog |
| Find Next | F3 | Find next occurrence |
| Find Previous | Shift+F3 | Find previous occurrence |
| Replace | Ctrl+H | Search and replace dialog |
| Go to Line | Ctrl+G | Jump to line number |
| Double-click select word | | Mouse word selection |
| **Enhance navigation** |
| Word navigation left | Ctrl+Left | Move caret by word |
| Word navigation right | Ctrl+Right | Move caret by word |
| Word selection left | Ctrl+Shift+Left | Extend selection by word |
| Word selection right | Ctrl+Shift+Right | Extend selection by word |
| Delete word left | Ctrl+Backspace | Delete to previous word boundary |
| Delete word right | Ctrl+Delete | Delete to next word boundary |
| Triple-click select line | | Mouse line selection (works with word wrap) |

---

## Pending Features

### Important (Standard Editor Features)

| Feature | Shortcut | Priority | Notes |
|---------|----------|----------|-------|
| **Large file support** | | P1 | Load large text files using memory mapped file |

### Nice to Have

| Feature | Priority | Notes |
|---------|----------|-------|
| **Status bar** | P3 | Show Ln/Col, encoding, line endings |
| **Line numbers** | P3 | Optional gutter display |
| **Zoom** | P4 | Ctrl+Plus/Minus (TUI limitations) |
| **Recent files** | P4 | File > Recent submenu |
| **Change Encoding** | P3 | Encoding submenu to change document encoding |

---

## Implementation Priority

1. ~~**Clipboard (Cut/Copy/Paste)** - Essential for any editor~~
2. ~~**Find/Replace** - Users expect this functionality~~
3. ~~**Word navigation** - Power user productivity~~
4. ~~**Go to Line** - Common for developers~~
5. **Status bar** - Provides context to users
5. **Large file support** - Provides context to users

---

## Technical Considerations

### Status Bar
- Terminal.Gui `StatusBar` view
- Update on caret movement
- Show: `Ln X, Col Y | UTF-8 | CRLF`
