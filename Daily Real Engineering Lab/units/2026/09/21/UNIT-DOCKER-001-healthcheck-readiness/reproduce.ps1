$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/Starter.csproj -- --reproduce
if ($LASTEXITCODE -ne 0) { throw 'Expected startup symptom was not reproduced.' }
Write-Host 'REPRODUCED: routing signal is positive while business traffic is not yet serviceable.'