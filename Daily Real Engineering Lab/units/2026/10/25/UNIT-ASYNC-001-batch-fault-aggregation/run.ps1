$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/BatchFaultLab.csproj -- --demo
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
} finally { Pop-Location }
