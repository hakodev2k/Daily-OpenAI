$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/LibraryHoldLab.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Run failed' }
