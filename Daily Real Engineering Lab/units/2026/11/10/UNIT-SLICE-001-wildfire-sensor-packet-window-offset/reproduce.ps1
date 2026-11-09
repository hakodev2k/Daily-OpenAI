$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/SensorGateway.csproj') -- --demo
$code = $LASTEXITCODE
if ($code -eq 1) { Write-Host 'REPRODUCED: starter fails one or more behavior fixtures'; exit 0 }
if ($code -eq 0) { Write-Error 'NOT REPRODUCED: starter already passes'; exit 1 }
Write-Error "Unexpected execution failure: $code"; exit 1
