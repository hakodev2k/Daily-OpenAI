$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/FerryStatus.csproj') -- --reproduce
if ($LASTEXITCODE -ne 0) { throw 'Expected failure not reproduced' }
