$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/RescueDecoder.csproj') -- verify
if ($LASTEXITCODE -ne 0) { throw "Learner workspace verification failed (exit $LASTEXITCODE)." }
