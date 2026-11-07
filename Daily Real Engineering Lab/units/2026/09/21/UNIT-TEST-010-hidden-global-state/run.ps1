$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/Starter.csproj -- isolated
if ($LASTEXITCODE -ne 0) { throw 'Isolated check failed unexpectedly.' }
Write-Host 'Now run ./reproduce.ps1 to reproduce the suite-only failure.'