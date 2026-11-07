$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet restore ./starter/CorrelationLab.csproj
    if ($LASTEXITCODE -ne 0) { throw 'Restore failed' }
    dotnet build ./starter/CorrelationLab.csproj --no-restore
    if ($LASTEXITCODE -ne 0) { throw 'Starter build failed' }
} finally { Pop-Location }
