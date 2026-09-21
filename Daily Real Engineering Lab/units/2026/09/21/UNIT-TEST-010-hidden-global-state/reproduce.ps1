$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/Starter.csproj -- isolated
if ($LASTEXITCODE -ne 0) { throw 'The isolated check must pass before reproducing the suite symptom.' }
dotnet run --project ./starter/Starter.csproj -- reproduce
if ($LASTEXITCODE -ne 0) { throw 'Expected ordered-suite symptom was not reproduced.' }
Write-Host 'REPRODUCED: isolated check passes, ordered suite exposes a deterministic failure.'