$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/RouteLab.csproj -- --demo
    if ($LASTEXITCODE -ne 0) { throw 'Demo failed' }
} finally { Pop-Location }
