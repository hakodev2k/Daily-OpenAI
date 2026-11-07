$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet restore ./starter/InventoryReconciliation.csproj
    exit $LASTEXITCODE
} finally { Pop-Location }
