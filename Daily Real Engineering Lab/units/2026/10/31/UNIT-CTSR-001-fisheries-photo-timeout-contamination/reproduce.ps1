$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet run --project (Join-Path $root 'starter/CoastalInspection.csproj') -- reproduce
if ($LASTEXITCODE -ne 0) { Write-Error 'Baseline symptom missing'; exit 1 }
Write-Host 'BASELINE SYMPTOM CONFIRMED'
