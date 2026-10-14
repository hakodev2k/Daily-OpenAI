$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet restore (Join-Path $PSScriptRoot 'starter/TelemetryLab.csproj')
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    dotnet restore (Join-Path $PSScriptRoot 'solution/TelemetryLab.csproj')
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
finally {
    Pop-Location
}
