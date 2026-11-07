$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet restore ./starter/RouteLab.csproj
    if ($LASTEXITCODE -ne 0) { throw 'Starter restore failed' }
    dotnet restore ./solution/RouteLab.csproj
    if ($LASTEXITCODE -ne 0) { throw 'Solution restore failed' }
} finally { Pop-Location }
