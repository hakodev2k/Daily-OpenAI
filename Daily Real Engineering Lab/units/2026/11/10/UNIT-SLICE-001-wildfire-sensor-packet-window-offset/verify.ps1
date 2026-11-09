$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/SensorGateway.csproj') -- --verify
if ($LASTEXITCODE -ne 0) { Write-Error 'Verification failed on learner-editable starter'; exit 1 }
Write-Host 'PASS: learner workspace satisfies fixture contracts'
