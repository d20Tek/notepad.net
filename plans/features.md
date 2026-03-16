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
| **StatusBar** |
| Status bar view | | Show Ln/Col, encoding, line endings |
| View > Status bar menu | | EditorSetting for status bar, toggle menu item to show hide |
| | | |
| Change File Encoding | File menu | Encoding submenu to change document encoding |
| Line numbers | View menu | Optional gutter display, toggle via View menu; current-line highlighting; click gutter to select line |
| Help > Getting Started | F1 | Opens project page in default browser |
| Help > About | Help menu | About dialog with app name, version, description, and copyright |
| Zoom In | View menu / Ctrl+= | Status bar hint to use terminal zoom; MessageBox if status bar hidden |
| Zoom Out | View menu / Ctrl+- | Same hint as Zoom In; also triggered by Ctrl+ScrollWheel |
| Recent files | File menu | File > Recent submenu > MRU file list |

---

## Pending Features

### Important (Standard Editor Features)

| Feature | Shortcut | Priority | Notes |
|---------|----------|----------|-------|
| **Large file support** | | P1 | Load large text files using memory mapped file |

### Nice to Have

| Feature | Priority | Notes |
|---------|----------|-------|
| **Insert/Overwrite mode** | P3 | Insert key toggle insert-overwrite mode |
| **Status Bar Enhancements** | P4 | show Word count, Text selection length |
| Fix Alt hot keys | P3 | Alt+F, Alt+E, etc. for menu access are not always working - getting swallowed by the editor |

---

## Implementation Priority

1. ~~**Clipboard (Cut/Copy/Paste)** - Essential for any editor~~
2. ~~**Find/Replace** - Users expect this functionality~~
3. ~~**Word navigation** - Power user productivity~~
4. ~~**Go to Line** - Common for developers~~
5. ~~**Status bar** - Provides context to users~~
6. ~~**Line numbers** - Optional gutter display~~
7. ~~**Help menu (About + Getting Started)**~~
8. ~~**Zoom** - View menu hint directing users to terminal zoom~~
9. **Large file support** - Provides context to users

---

## Technical Considerations

### Status Bar
- Terminal.Gui `StatusBar` view
- Update on caret movement
- Show: `Ln X, Col Y | UTF-8 | CRLF`
