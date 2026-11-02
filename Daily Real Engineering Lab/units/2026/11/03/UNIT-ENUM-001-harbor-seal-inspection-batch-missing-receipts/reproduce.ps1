$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/SealImportLab.csproj -- --reproduce
    if ($LASTEXITCODE -ne 0) { throw "Starter reproduction failed: $LASTEXITCODE" }
} finally { Pop-Location }
