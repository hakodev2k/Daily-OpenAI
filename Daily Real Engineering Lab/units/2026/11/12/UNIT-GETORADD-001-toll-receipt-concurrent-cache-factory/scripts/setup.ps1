$ErrorActionPreference = 'Stop'
dotnet --version
if ($LASTEXITCODE -ne 0) { throw 'dotnet SDK required' }
dotnet restore (Join-Path $PSScriptRoot '../starter/TollReceiptLab.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Restore failed' }
