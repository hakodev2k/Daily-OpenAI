$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/MuseumAudit.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Starter failed' }
