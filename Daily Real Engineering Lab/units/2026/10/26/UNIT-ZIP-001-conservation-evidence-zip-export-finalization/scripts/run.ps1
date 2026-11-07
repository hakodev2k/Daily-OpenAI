$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
dotnet run --project (Join-Path $root 'starter/EvidenceExportLab.csproj') -- --diagnose
if ($LASTEXITCODE -ne 0) { throw 'Diagnostic run failed.' }
