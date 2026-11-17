$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/WarehouseGate.csproj"
if ($LASTEXITCODE -eq 0) { throw 'Reproduction did not detect the intended failure' }
Write-Host 'EXPECTED FAILURE reproduced'
