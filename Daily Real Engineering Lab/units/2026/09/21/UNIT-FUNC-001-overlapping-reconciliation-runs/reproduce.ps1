$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/Starter.csproj -- --reproduce
if ($LASTEXITCODE -ne 0) { throw 'Starter did not reproduce the expected overlapping business execution.' }
Write-Host 'REPRODUCED: one logical window produced two business executions.'