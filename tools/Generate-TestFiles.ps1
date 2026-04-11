# Generate-TestFiles.ps1
# Creates sample text files of various sizes for manual testing of LargeTextStorage.
# Usage: .\tools\Generate-TestFiles.ps1 [-OutputDir <path>] [-Sizes <MB list>]
#
# Examples:
#   .\tools\Generate-TestFiles.ps1                          # default sizes to tools\TestFiles
#   .\tools\Generate-TestFiles.ps1 -Sizes 1,10              # only 1 MB and 10 MB
#   .\tools\Generate-TestFiles.ps1 -OutputDir C:\temp\files  # custom output directory

param(
    [string]$OutputDir = (Join-Path $PSScriptRoot "TestFiles"),
    [int[]]$Sizes = @(1, 10, 50, 100)
)

$sampleLines = @(
    "The quick brown fox jumps over the lazy dog."
    "Lorem ipsum dolor sit amet, consectetur adipiscing elit."
    "Pack my box with five dozen liquor jugs."
    "How vexingly quick daft zebras jump!"
    "The five boxing wizards jump quickly."
    "Bright vixens jump; dozy fowl quack."
    "Sphinx of black quartz, judge my vow."
    "Two driven jocks help fax my big quiz."
    "Jackdaws love my big sphinx of quartz."
    "     Indented line with leading spaces."
    ""
    "A line after an empty line."
    "UPPERCASE LINE FOR VARIETY."
    "MixedCase Line With Multiple Words Here."
    "1234567890 - a line with numbers and symbols: @#$%^&*()"
    "Short."
    "A somewhat longer line that contains more text to simulate realistic paragraph-style content in a plain text file."
    "	Tab-indented line with a tab character at the start."
    "Line with trailing spaces   "
    "another line. and another sentence. periods everywhere."
)

if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

foreach ($sizeMB in $Sizes) {
    $targetBytes = [long]$sizeMB * 1024 * 1024
    $fileName = "test-${sizeMB}mb.txt"
    $filePath = Join-Path $OutputDir $fileName

    Write-Host "Generating $fileName ($sizeMB MB)..." -NoNewline

    $stream = [System.IO.StreamWriter]::new($filePath, $false, [System.Text.UTF8Encoding]::new($false))
    try {
        $written = 0L
        $lineIndex = 0
        $lineCount = $sampleLines.Count

        while ($written -lt $targetBytes) {
            $line = "$($lineIndex + 1): $($sampleLines[$lineIndex % $lineCount])"
            $stream.WriteLine($line)
            $written += [System.Text.Encoding]::UTF8.GetByteCount($line) + 2  # +2 for CRLF
            $lineIndex++
        }
    }
    finally {
        $stream.Dispose()
    }

    $actualSize = (Get-Item $filePath).Length
    $actualMB = [math]::Round($actualSize / 1MB, 2)
    Write-Host " done ($actualMB MB, $lineIndex lines)"
}

Write-Host ""
Write-Host "Files generated in: $OutputDir"
Write-Host "Default large file threshold is 10 MB (10,485,760 bytes)."
Write-Host "Files at or above that size will use the LargeTextStorage path."
