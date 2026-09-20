$ErrorActionPreference = 'Stop'
dotnet build ./starter/PathBaseLab.csproj --nologo | Out-Null
dotnet run --project ./starter/PathBaseLab.csproj --no-build -- --reproduce
if ($LASTEXITCODE -ne 0) { throw 'Starter no longer reproduces the expected public-boundary failure.' }
Write-Host 'REPRODUCED: generated navigation escaped the mounted application boundary.'