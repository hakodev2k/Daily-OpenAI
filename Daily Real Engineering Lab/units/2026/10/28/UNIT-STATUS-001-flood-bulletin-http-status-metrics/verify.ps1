$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot
dotnet run --project (Join-Path $PSScriptRoot "starter/TelemetryLab.csproj") -- --verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
