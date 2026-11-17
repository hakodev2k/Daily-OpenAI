$ErrorActionPreference = 'Stop'
dotnet --version
dotnet restore (Join-Path $PSScriptRoot '../starter/SupplyImport.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Restore failed' }
