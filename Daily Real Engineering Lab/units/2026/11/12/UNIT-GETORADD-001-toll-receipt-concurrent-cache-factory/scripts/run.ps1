$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/TollReceiptLab.csproj') -- --run
if ($LASTEXITCODE -ne 0) { throw 'Run failed' }
