$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/TollReceiptLab.csproj') -- --verify
if ($LASTEXITCODE -ne 0) { throw 'Learner starter contract not satisfied' }
