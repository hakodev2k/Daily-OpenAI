$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/Lab.csproj') -- --reproduce
if ($LASTEXITCODE -ne 0) { throw 'Starter did not reproduce the expected failure' }
Write-Host 'REPRODUCTION PASSED'
