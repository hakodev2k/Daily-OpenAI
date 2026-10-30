$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/InventoryReconciliation.csproj -- reproduce
    exit $LASTEXITCODE
} finally { Pop-Location }
