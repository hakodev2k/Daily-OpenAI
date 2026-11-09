$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try { dotnet run --project ./starter/RescueLab.csproj --; exit $LASTEXITCODE } finally { Pop-Location }
