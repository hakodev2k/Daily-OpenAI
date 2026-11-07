$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/InventoryReconciliation.csproj -- verify
    exit $LASTEXITCODE
} finally { Pop-Location }
