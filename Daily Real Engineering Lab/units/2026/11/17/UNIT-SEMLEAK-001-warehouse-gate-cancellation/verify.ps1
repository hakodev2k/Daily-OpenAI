$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/WarehouseGate.csproj"
if ($LASTEXITCODE -ne 0) { throw 'Starter still violates gate invariant' }
Write-Host 'VERIFIED learner starter'
