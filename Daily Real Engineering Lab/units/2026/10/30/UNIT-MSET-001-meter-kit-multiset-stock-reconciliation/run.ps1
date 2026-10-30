$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/InventoryReconciliation.csproj -- demo
    exit $LASTEXITCODE
} finally { Pop-Location }
