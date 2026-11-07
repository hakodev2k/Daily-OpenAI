$ErrorActionPreference = 'Stop'
$unitRoot = Split-Path -Parent $PSScriptRoot
Push-Location $unitRoot
try {
    dotnet run --project "harness/IncidentHarness.csproj" -- --verify
    $result = $LASTEXITCODE
}
finally {
    Pop-Location
}
exit $result
