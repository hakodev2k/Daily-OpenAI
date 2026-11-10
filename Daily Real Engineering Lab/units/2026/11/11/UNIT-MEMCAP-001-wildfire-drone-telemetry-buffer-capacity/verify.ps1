$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/TelemetryLab.csproj'
dotnet run --project $project -- verify
if ($LASTEXITCODE -ne 0) { throw 'Verification failed: frame contract remains broken.' }
Write-Host 'Verification passed: all frame sizes accepted.'
