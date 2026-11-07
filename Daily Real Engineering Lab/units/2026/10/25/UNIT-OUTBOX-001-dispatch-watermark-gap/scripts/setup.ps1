$ErrorActionPreference = 'Stop'
$unitRoot = Split-Path -Parent $PSScriptRoot
Push-Location $unitRoot
try {
    dotnet restore "harness/IncidentHarness.csproj"
    if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed" }
    dotnet build "harness/IncidentHarness.csproj" --no-restore
    if ($LASTEXITCODE -ne 0) { throw "dotnet build failed" }
}
finally {
    Pop-Location
}
