# Run from the repository root with PowerShell.
$ErrorActionPreference = 'Stop'
$outputRoot = Join-Path ([IO.Path]::GetTempPath()) ("crossapp-publish-" + [guid]::NewGuid())
foreach ($mode in 'framework', 'self-contained', 'single', 'trimmed') {
    $folder = Join-Path $outputRoot $mode
    $options = @()
    if ($mode -eq 'single') { $options += '-p:PublishSingleFile=true' }
    if ($mode -eq 'trimmed') { $options += '-p:PublishTrimmed=true' }
    $bundled = ($mode -ne 'framework').ToString().ToLowerInvariant()
    dotnet publish src/Cli -c Release -f net10.0 -r win-x64 --self-contained $bundled -o $folder @options
    if ($LASTEXITCODE) { throw "Publish failed: $mode" }
    $files = @(Get-ChildItem $folder -Recurse -File)
    Push-Location $folder
    try {
        $json = & .\Cli.exe --json
        if ($LASTEXITCODE) { throw "Launch failed: $mode" }
        $report = $json | ConvertFrom-Json
        if ($report.ReportedRid -ne 'win-x64') { throw "Invalid report: $mode" }
        "$mode | $($files.Count) files | $(($files | Measure-Object Length -Sum).Sum) bytes | $($report.BuildNote)"
    } finally { Pop-Location }
}
