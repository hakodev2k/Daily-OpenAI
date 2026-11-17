$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/BuoyStream.csproj') -- reproduce
if ($LASTEXITCODE -ne 0) { throw 'Reproduction failed' }
