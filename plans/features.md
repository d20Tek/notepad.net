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
| Cut | Ctrl+X | Remove selection, copy to clipboard |
| Copy | Ctrl+C | Copy selection to clipboard |
| Paste | Ctrl+V | Insert clipboard content |

---

## Pending Features

### Important (Standard Editor Features)

| Feature | Shortcut | Priority | Notes |
|---------|----------|----------|-------|
| **Find** | Ctrl+F | P1 | Search dialog |
| **Find Next** | F3 | P1 | Find next occurrence |
| **Find Previous** | Shift+F3 | P1 | Find previous occurrence |
| **Replace** | Ctrl+H | P1 | Search and replace dialog |
| **Go to Line** | Ctrl+G | P1 | Jump to line number |
| **Large file support** | | P1 | Load large text files using memory mapped file |

### Navigation Enhancements

| Feature | Shortcut | Priority | Notes |
|---------|----------|----------|-------|
| Word navigation left | Ctrl+Left | P2 | Move caret by word |
| Word navigation right | Ctrl+Right | P2 | Move caret by word |
| Word selection left | Ctrl+Shift+Left | P2 | Extend selection by word |
| Word selection right | Ctrl+Shift+Right | P2 | Extend selection by word |
| Delete word left | Ctrl+Backspace | P2 | Delete previous word |
| Delete word right | Ctrl+Delete | P2 | Delete next word |

### Nice to Have

| Feature | Priority | Notes |
|---------|----------|-------|
| **Status bar** | P3 | Show Ln/Col, encoding, line endings |
| **Line numbers** | P3 | Optional gutter display |
| **Double-click select word** | P3 | Mouse word selection |
| **Triple-click select line** | P3 | Mouse line selection |
| **Zoom** | P4 | Ctrl+Plus/Minus (TUI limitations) |
| **Recent files** | P4 | File > Recent submenu |
| **Print** | P4 | Print document |

---

## Implementation Priority

1. **Clipboard (Cut/Copy/Paste)** - Essential for any editor
2. **Find/Replace** - Users expect this functionality
3. **Word navigation** - Power user productivity
4. **Go to Line** - Common for developers
5. **Status bar** - Provides context to users

---

## Technical Considerations

### Clipboard
- Terminal.Gui has `Clipboard` class for cross-platform clipboard access
- Need to handle selection state for cut/copy
- Paste should replace selection if active

### Find/Replace
- Consider highlight all matches
- Incremental search (highlight as you type)
- Case sensitivity option
- Wrap around option

### Status Bar
- Terminal.Gui `StatusBar` view
- Update on caret movement
- Show: `Ln X, Col Y | UTF-8 | CRLF`
