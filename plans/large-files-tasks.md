# Large File Support

## Overview
Add the ability to efficiently load and work with large text files using memory-mapped files.
Currently, `SimpleTextStorage.Load` calls `ReadToEnd()` which reads the entire file into a
single managed string, then splits it into `List<TextLine>`. For files exceeding tens of
megabytes this causes excessive memory allocation and long blocking loads.

The solution introduces a **two-tier loading strategy**: files below a configurable threshold
(default 10 MB) continue to use the existing `SimpleTextStorage` path unchanged; files at or
above the threshold use a new `LargeTextStorage` that memory-maps the file, builds a
lightweight line-offset index, and materializes `TextLine` objects lazily on demand.

Once the file is loaded (either path), the document is a fully editable `IDocument` backed by
the same `List<TextLine>` structure — so **all existing editing, undo, search, rendering, and
save logic remains unchanged**. The key difference is *how* the initial `List<TextLine>` is
populated.

**Goal**: Open a 500 MB+ UTF-8 text file without `OutOfMemoryException` and with a
responsive UI during loading.

---

## Architecture Design

### Loading strategy selection
```
DocumentFactory.Load(filePath)
  → FileInfo.Length check
  → if < threshold: SimpleTextStorage.Load(stream)            [existing path]
  → if ≥ threshold: LargeTextStorage.Load(filePath, progress)  [new path]
  → DocumentData → Document(List<TextLine>)                    [shared from here]
```

### `LargeTextStorage` internals
```
LargeTextStorage.Load(filePath, progress?, cancellation?)
  1. Open MemoryMappedFile from filePath (read-only)
  2. Create MemoryMappedViewAccessor (or ViewStream) over entire file
  3. Detect encoding from first 4 bytes (reuse EncodingDetector)
  4. Phase 1 — Index scan:
     - Walk the byte buffer sequentially, recording the byte offset of each
       line start into a List<long> (or long[] grown in chunks)
     - Detect line ending style (CRLF/LF/CR) from first newline found
     - Report progress periodically (every N lines or every N bytes)
  5. Phase 2 — Materialize lines:
     - For each indexed line span, decode bytes → string → TextLine
     - Build List<TextLine> of exact size (capacity = line count)
     - Report progress periodically
  6. Return DocumentData(lines, encoding, lineEndingStyle)
  7. Dispose MemoryMappedFile + accessor
```

### Progress reporting & cancellation
A new `ILoadProgress` interface allows the UI to display a progress dialog during
large-file loads. `FileOpenCommand` (and `OpenRecentCommand`) wrap the load in a
progress dialog when the file size exceeds the threshold. A `CancellationToken`
parameter allows the user to cancel a long-running load.

### Data flow — unchanged after load
```
DocumentData → DocumentFactory.Create(data) → Document(List<TextLine>)
  → EditorSession.ReplaceDocument(newDoc)
  → all editing, undo, search, rendering works on List<TextLine> as before
```

### Save — unchanged
Save always writes the full `List<TextLine>` through `SimpleTextStorage.Save`. No
memory-mapped write path is needed — saves are already fast and the document is fully
in memory once loaded and edited.

### Configuration
`EditorSettings` gains a `LargeFileThresholdBytes` property (default `10_485_760` =
10 MB). This is persisted in `editor-settings.json` and can be adjusted but is not
exposed in the UI.

---

## Phase 1: Core — `LargeTextStorage` and Loading Infrastructure

### Task 1.1: Create `ILoadProgress` interface
File: `src\D20Tek.Notepad.Core\Storage\ILoadProgress.cs`
- [x] Define `public interface ILoadProgress`
- [x] Add `void Report(long bytesProcessed, long totalBytes)` method
- [x] Add `static ILoadProgress None { get; }` returning a no-op implementation

### Task 1.2: Create `LargeTextStorage` class
File: `src\D20Tek.Notepad.Core\Storage\LargeTextStorage.cs`
- [x] Define `internal sealed class LargeTextStorage`
- [x] Add `public DocumentData Load(string filePath, ILoadProgress? progress = null,
      CancellationToken cancellation = default)`:
  - Open `MemoryMappedFile.CreateFromFile(filePath, FileMode.Open, null, 0,
    MemoryMappedFileAccess.Read)` in a `using` block
  - Create `MemoryMappedViewAccessor` with `MemoryMappedFileAccess.Read`
  - Detect encoding via first 4 bytes using `EncodingDetector` pattern (read BOM bytes
    from the accessor)
  - Call `BuildLineIndex(accessor, fileLength, preambleLength, progress, cancellation)`
    to get `List<long> lineOffsets` and `LineEndingStyle`
  - Call `MaterializeLines(accessor, lineOffsets, fileLength, encoding, progress,
    cancellation)` to get `List<TextLine>`
  - Return `new DocumentData(lines, encoding, lineEndingStyle)`
- [x] Add `private static (List<long> Offsets, LineEndingStyle Style) BuildLineIndex(...)`:
  - Walk bytes from `preambleLength` to end of file
  - Use `accessor.ReadByte(offset)` or read chunks via `ReadArray<byte>` for performance
  - Record byte offset of each line start (first offset = preambleLength)
  - Detect `\r\n` vs `\n` vs `\r` for `LineEndingStyle` (from first newline found)
  - Call `cancellation.ThrowIfCancellationRequested()` periodically
  - Report progress periodically via `progress?.Report(offset, fileLength)`
- [x] Add `private static List<TextLine> MaterializeLines(...)`:
  - Pre-allocate `List<TextLine>(lineOffsets.Count)`
  - For each consecutive pair of offsets, compute byte length of line (excluding line
    ending bytes)
  - Read byte span via `accessor.ReadArray<byte>` and decode with `encoding.GetString`
  - Add `new TextLine(decodedString)` to the list
  - Handle last line (from last offset to end of file)
  - Report progress periodically

### Task 1.3: Add `LargeFileThresholdBytes` to `EditorSettings`
File: `src\D20Tek.Notepad.ViewModel\EditorSettings.cs`
- [x] Add `long LargeFileThresholdBytes = 10_485_760` parameter to the record
- [x] Ensure `EditorSettings.Default` includes this value

### Task 1.4: Modify `DocumentFactory.Load` to select storage strategy
File: `src\D20Tek.Notepad.Core\Document\DocumentFactory.cs`
- [x] Change `Load(string filePath)` signature to
      `Load(string filePath, long largeFileThreshold = 10_485_760,
      ILoadProgress? progress = null, CancellationToken cancellation = default)`
- [x] Add file-size check: `new FileInfo(filePath).Length`
- [x] If below threshold: use existing `SimpleTextStorage` path (unchanged)
- [x] If at or above threshold: use `new LargeTextStorage().Load(filePath, progress,
      cancellation)`
- [x] Both paths produce `DocumentData` → `Create(data)` → `IDocument` (unchanged)

### Task 1.5: Update `IDocumentFactory` interface
File: `src\D20Tek.Notepad.Core\Document\IDocumentFactory.cs`
- [x] Update `Load` signature to match `DocumentFactory.Load` (add optional parameters
      for threshold, progress, and cancellation)

### Task 1.6: Unit tests for `LargeTextStorage`
File: `tests\D20Tek.Notepad.Core.UnitTests\Storage\LargeTextStorageTests.cs`
- [x] `Load_SmallUtf8File_ReturnsCorrectLinesAndEncoding`
- [x] `Load_FileWithCrLfEndings_DetectsLineEndingStyle`
- [x] `Load_FileWithLfEndings_DetectsLineEndingStyle`
- [x] `Load_Utf8BomFile_StripsAndDetectsEncoding`
- [x] `Load_EmptyFile_ReturnsSingleEmptyLine`
- [x] `Load_SingleLineFile_ReturnsSingleLine`
- [x] `Load_ReportsProgress`
- [x] `Load_CancellationRequested_ThrowsOperationCanceledException`

### Task 1.7: Unit tests for updated `DocumentFactory.Load`
File: `tests\D20Tek.Notepad.Core.UnitTests\Document\DocumentFactoryTests.cs`
- [x] `Load_FileBelowThreshold_UsesSimpleStorage`
- [x] `Load_FileAtOrAboveThreshold_UsesLargeStorage`
- [x] Update any existing tests affected by the signature change

### Task 1.8: Unit tests for `EditorSettings.LargeFileThresholdBytes`
File: `tests\D20Tek.Notepad.ViewModel.UnitTests\EditorSettingsTests.cs`
- [x] Update existing tests to include `LargeFileThresholdBytes` assertions
- [x] Verify default value is `10_485_760`

---

## Phase 2: UI — Progress Dialog and Command Integration

### Task 2.1: Create `LoadProgressDialog` in Tui project
File: `src\D20Tek.Notepad.Tui\Dialogs\LoadProgressDialog.cs`
- [x] Define `internal sealed class LoadProgressDialog : Dialog` (Terminal.Gui v1 pattern)
- [x] Display a centered dialog with:
  - Title: `"Loading Large File"`
  - A `Label` showing `"Loading {fileName}..."` 
  - A `ProgressBar` bound to `ILoadProgress.Report` updates
  - A `"Cancel"` button wired to `CancellationTokenSource.Cancel()`
- [x] Implement `ILoadProgress` to update the progress bar via
      `Application.MainLoop.Invoke()` for thread-safe UI updates
- [x] Expose `CancellationToken Token` property for callers
- [x] Expose a factory method: `static LoadProgressDialog Create(string fileName)`

### Task 2.2: Modify `FileOpenCommand` to handle large files
File: `src\D20Tek.Notepad.Tui\Commands\FileOpenCommand.cs`
- [x] After getting `filePath` from `OpenDialog`, check file size against
      `viewModel.Settings.LargeFileThresholdBytes`
- [x] If below threshold: use existing `factory.Load(filePath)` path (unchanged)
- [x] If at/above threshold:
  - Create `LoadProgressDialog` with the file name
  - Run the load on a background thread (`Task.Run`) passing the dialog's
    `ILoadProgress` and `CancellationToken`
  - Show the dialog modally (`Application.Run(dialog)`)
  - On completion, close dialog and apply document to session
  - On cancellation, show no error — just return
  - On exception, show `MessageBox.ErrorQuery`
- [x] Extract shared document-apply logic into a private helper to avoid duplication

### Task 2.3: Modify `OpenRecentCommand` to handle large files
File: `src\D20Tek.Notepad.Tui\Commands\OpenRecentCommand.cs`
- [x] Apply the same large-file check + progress dialog pattern as `FileOpenCommand`
- [x] Extract any shared helper from Task 2.2 if both commands need the same logic
      (consider a `FileLoadHelper` static class in the Commands folder)

### Task 2.4: Create `FileLoadHelper` for shared load-and-apply logic
File: `src\D20Tek.Notepad.Tui\Commands\FileLoadHelper.cs`
- [x] Define `internal static class FileLoadHelper`
- [x] Add `public static bool LoadAndApply(EditorViewModel viewModel, string filePath)`:
  - Check file size vs `viewModel.Settings.LargeFileThresholdBytes`
  - Small file: synchronous `factory.Load(filePath)` → apply to session → return true
  - Large file: create `LoadProgressDialog`, run background load, show dialog, apply
    result on completion, handle cancellation/errors → return true/false
- [x] Both `FileOpenCommand.Execute` and `OpenRecentCommand.Execute` call this helper
      after the unsaved-changes check and file selection

---

## Phase 3: Performance Optimization

### Task 3.1: Use chunked byte reading in `BuildLineIndex`
File: `src\D20Tek.Notepad.Core\Storage\LargeTextStorage.cs`
- [x] Instead of `ReadByte` one at a time, read in 64 KB (or configurable) chunks
      using `accessor.ReadArray<byte>(offset, buffer, 0, chunkSize)`
- [x] Process the chunk buffer to find newline byte positions
- [x] Handle newline characters that span chunk boundaries (`\r` at end of chunk,
      `\n` at start of next)

### Task 3.2: Use `Span<byte>` and `unsafe` accessor for materialization
File: `src\D20Tek.Notepad.Core\Storage\LargeTextStorage.cs`
- [x] For `MaterializeLines`, consider using `MemoryMappedViewAccessor.SafeMemoryMappedViewHandle`
      with `AcquirePointer` / `ReleasePointer` to get direct byte pointer access
- [x] Use `Encoding.GetString(ReadOnlySpan<byte>)` for zero-copy decoding
- [x] Only apply if benchmarks show measurable improvement over `ReadArray`

### Task 3.3: Benchmark `LargeTextStorage` vs `SimpleTextStorage`
- [x] Create a benchmark (manual) comparing load times:
  - 1 MB file, 10 MB file, 100 MB file, 500 MB file
  - Measure wall-clock time and peak memory allocation
  - Document results in this file or a separate benchmark report

---

## Phase 4: Edge Cases and Robustness

### Task 4.1: Handle non-UTF-8 encodings in `LargeTextStorage`
File: `src\D20Tek.Notepad.Core\Storage\LargeTextStorage.cs`
- [ ] UTF-8 (with and without BOM): variable-width — line-index scan must look for
      `0x0A` byte (which cannot appear in a multi-byte UTF-8 sequence except as LF)
- [ ] UTF-16 LE / BE: line scan must look for `\n` as a two-byte sequence at aligned
      positions. Use `EncodingDetector` BOM result to select the scan strategy
- [ ] Fallback: if encoding is not UTF-8/UTF-16, fall back to `SimpleTextStorage`
      (read entire file) — non-UTF encodings are rare for large files

### Task 4.2: Handle files with no trailing newline
File: `src\D20Tek.Notepad.Core\Storage\LargeTextStorage.cs`
- [ ] Ensure the last line (from last newline to EOF) is captured even if there is no
      trailing newline character
- [ ] Add unit test: `Load_FileWithNoTrailingNewline_ReturnsCorrectLastLine`

### Task 4.3: Handle very long lines (lines > 10 MB)
File: `src\D20Tek.Notepad.Core\Storage\LargeTextStorage.cs`
- [ ] No special handling needed at the storage layer — `string` supports up to 2 GB
- [ ] Document that rendering performance for extremely long lines is a separate concern

### Task 4.4: Handle file-in-use / locked files
File: `src\D20Tek.Notepad.Core\Storage\LargeTextStorage.cs`
- [ ] Use `FileShare.Read` when creating the `MemoryMappedFile` to allow concurrent
      readers
- [ ] Catch `IOException` and surface a clear error message
- [ ] Add unit test: `Load_LockedFile_ThrowsIOException` (or meaningful error)

### Task 4.5: Unit tests for edge cases
File: `tests\D20Tek.Notepad.Core.UnitTests\Storage\LargeTextStorageTests.cs`
- [ ] `Load_FileWithMixedLineEndings_UsesFirstDetectedStyle`
- [ ] `Load_BinaryFileWithNullBytes_LoadsWithoutCrash`
- [ ] `Load_FileExactlyAtThreshold_UsesLargeStorage` (via DocumentFactory)

---

## Phase 5: Testing and Validation

### Task 5.1: Integration test — open large file via `DocumentFactory`
File: `tests\D20Tek.Notepad.Core.UnitTests\Document\DocumentFactoryTests.cs`
- [x] Create a temporary file > threshold size, load via `DocumentFactory.Load`
      with threshold set to small value (e.g., 100 bytes) to force large-file path
- [x] Verify line count, first line content, last line content, encoding, line ending

### Task 5.2: Verify save round-trip for large-file-loaded documents
File: `tests\D20Tek.Notepad.Core.UnitTests\Document\DocumentFactoryTests.cs`
- [x] Load a file via large-file path
- [x] Save via `DocumentFactory.Save`
- [x] Re-load and verify content matches original

### Task 5.3: Verify editing operations work after large-file load
File: `tests\D20Tek.Notepad.Core.UnitTests\Editing\EditorSessionTests.Commands.cs`
- [x] Load a document via large-file path
- [x] Create `EditorSession` with the loaded document
- [x] Perform InsertText, Backspace, Delete, Undo, Redo
- [x] Verify document state is correct after each operation

### Task 5.4: Update `features.md`
File: `plans\features.md`
- [x] Move **Large file support** from Pending to Implemented features table

---

## File Summary

| File | Action | Purpose |
|------|--------|---------|
| `src\D20Tek.Notepad.Core\Storage\ILoadProgress.cs` | Create | Progress callback interface |
| `src\D20Tek.Notepad.Core\Storage\LargeTextStorage.cs` | Create | Memory-mapped file loader |
| `src\D20Tek.Notepad.Core\Storage\ITextStorageStrategy.cs` | No change | Existing interface — not modified |
| `src\D20Tek.Notepad.Core\Document\IDocumentFactory.cs` | Modify | Add optional params to Load |
| `src\D20Tek.Notepad.Core\Document\DocumentFactory.cs` | Modify | Size-based strategy selection |
| `src\D20Tek.Notepad.ViewModel\EditorSettings.cs` | Modify | Add LargeFileThresholdBytes |
| `src\D20Tek.Notepad.Tui\Dialogs\LoadProgressDialog.cs` | Create | Progress dialog for large loads |
| `src\D20Tek.Notepad.Tui\Commands\FileLoadHelper.cs` | Create | Shared load-and-apply logic |
| `src\D20Tek.Notepad.Tui\Commands\FileOpenCommand.cs` | Modify | Use FileLoadHelper |
| `src\D20Tek.Notepad.Tui\Commands\OpenRecentCommand.cs` | Modify | Use FileLoadHelper |
| `tests\...\LargeTextStorageTests.cs` | Create | Storage unit tests |
| `tests\...\DocumentFactoryTests.cs` | Modify | Threshold-based loading tests |
| `tests\...\EditorSettingsTests.cs` | Modify | New property tests |
| `tests\...\EditorSessionTests.Commands.cs` | Modify | Post-load editing tests |
