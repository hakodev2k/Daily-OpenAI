$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/BuoyStream.csproj') -- verify
if ($LASTEXITCODE -ne 0) { throw 'Verification failed' }
