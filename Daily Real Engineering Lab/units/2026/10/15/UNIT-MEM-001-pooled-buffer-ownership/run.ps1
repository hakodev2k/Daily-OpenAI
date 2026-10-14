$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --no-launch-profile --project (Join-Path $PSScriptRoot 'starter/TelemetryLab.csproj') -- run
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
finally {
    Pop-Location
}
