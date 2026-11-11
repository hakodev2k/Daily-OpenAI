$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/MapResumeLab.csproj') -- reproduce
if ($LASTEXITCODE -ne 0) { throw 'reproduction failed' }
