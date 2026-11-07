$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/SealImportLab.csproj -- --reproduce
    if ($LASTEXITCODE -ne 0) { throw "Lab run failed: $LASTEXITCODE" }
} finally { Pop-Location }
