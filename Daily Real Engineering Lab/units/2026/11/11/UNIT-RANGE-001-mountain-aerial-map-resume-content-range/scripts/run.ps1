$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/MapResumeLab.csproj') -- run
if ($LASTEXITCODE -ne 0) { throw 'run failed' }
