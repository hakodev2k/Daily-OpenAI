$ErrorActionPreference = 'Stop'
$unitRoot = Split-Path -Parent $PSScriptRoot
Push-Location $unitRoot
try {
    dotnet run --project "harness/IncidentHarness.csproj" -- --demo
    $result = $LASTEXITCODE
}
finally {
    Pop-Location
}
exit $result
