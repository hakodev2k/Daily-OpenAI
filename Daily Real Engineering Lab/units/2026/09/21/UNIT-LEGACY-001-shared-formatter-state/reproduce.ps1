$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/Starter.csproj -- --sequential
if ($LASTEXITCODE -ne 0) { throw 'Baseline is already broken.' }
dotnet run --project ./starter/Starter.csproj -- --reproduce
if ($LASTEXITCODE -ne 0) { throw 'Expected parallel symptom was not reproduced.' }
Write-Host 'REPRODUCED: sequential behavior passes while coordinated parallel rendering violates isolation.'