$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/MapResumeLab.csproj') -- verify
if ($LASTEXITCODE -ne 0) { throw 'learner verification failed' }
