$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    dotnet run --project ./starter/ManifestAudit.csproj -- reproduce
    $result = $LASTEXITCODE
} finally {
    Pop-Location
}
if ($result -ne 0) { throw "Baseline symptom not reproduced (exit $result). Check original starter and .NET SDK." }
Write-Host "PASS: Original starter symptoms reproduced."
