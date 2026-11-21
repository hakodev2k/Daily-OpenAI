$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
dotnet run --project (Join-Path $root 'starter/DeviceLab.csproj') -- --contract-check
if ($LASTEXITCODE -eq 0) { throw 'Unexpected: starter passed the contract; cannot reproduce failure' }
Write-Host 'REPRODUCED: starter fails contract checks (expected)'
