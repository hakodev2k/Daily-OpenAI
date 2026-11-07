$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
dotnet --version
if ($LASTEXITCODE -ne 0) { throw 'A .NET 8 SDK is required.' }
dotnet restore (Join-Path $root 'starter/EvidenceExportLab.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Restore failed.' }
