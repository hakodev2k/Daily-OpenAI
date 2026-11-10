$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/FerryStatus.csproj') -- --verify
if ($LASTEXITCODE -ne 0) { throw 'Learner starter still violates contract' }
