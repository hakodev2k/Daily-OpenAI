$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/TollReceiptLab.csproj') -- --reproduce
if ($LASTEXITCODE -ne 0) { throw 'Incident not reproduced' }
