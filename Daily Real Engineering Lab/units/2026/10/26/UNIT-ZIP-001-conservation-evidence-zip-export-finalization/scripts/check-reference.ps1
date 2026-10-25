$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
dotnet run --project (Join-Path $root 'solution/EvidenceExportLab.csproj') -- --verify
if ($LASTEXITCODE -ne 0) { throw 'Reference self-check FAILED.' }
