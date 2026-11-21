$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
dotnet run --project (Join-Path $root 'starter/DeviceLab.csproj') -- --contract-check
if ($LASTEXITCODE -ne 0) { throw 'Learner starter fails contract checks' }
Write-Host 'VERIFIED: learner-editable starter passes all contract checks'
