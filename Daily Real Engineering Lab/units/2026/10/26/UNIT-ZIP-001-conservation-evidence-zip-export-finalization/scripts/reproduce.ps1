$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
dotnet run --project (Join-Path $root 'starter/EvidenceExportLab.csproj') -- --reproduce
if ($LASTEXITCODE -ne 0) { throw 'Expected starter failure was NOT reproduced; check the unmodified starter and logs.' }
