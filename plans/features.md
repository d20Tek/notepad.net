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
| Insert/Overwrite mode | Insert key / View menu | Toggle insert-overwrite mode; OVR/INS in status bar; block cursor in overwrite |
| | | |
| Large file support | | P1 | Load large text files using memory mapped file |

---

## Pending Features

### Nice to Have

| Feature | Shortcut | Effort | Rationale |
|---------|----------|--------|-----------|
| Duplicate Line/Selection | Ctrl+Shift+D | Low | Copy current line (or selection) below caret. Very common text manipulation shortcut — straightforward to implement via EditorSession using existing insert/split logic. |
| Move Line Up/Down | Alt+Up / Alt+Down | Low | Swap current line with the one above/below. Operates directly on Lines list with an undo operation. Avoids the cut-paste dance users would otherwise do. |
| Indent/Outdent Selection | Tab / Shift+Tab (multi-line) | Medium | When multiple lines are selected, Tab prepends indent to each line and Shift+Tab removes one level. Your Tab key already inserts, but multi-line selection indentation is a distinct operation users expect. Needs a bulk ReplaceRangeOperation for undo support. |
| Status Bar Enhancements | | | word count and selection length. StatusDetails just needs two new fields. The infrastructure is fully in place. |
| Trim Trailing Whitespace | Edit menu | Low | Single command that strips trailing spaces/tabs from every line. Simple iteration over Lines with a compound undo operation. Handy for cleaning up text files before saving. |

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
9. ~~**Large file support** - Provides context to users~~
