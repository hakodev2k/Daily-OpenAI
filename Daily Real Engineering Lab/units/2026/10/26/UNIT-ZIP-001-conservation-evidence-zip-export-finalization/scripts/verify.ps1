$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
dotnet run --project (Join-Path $root 'starter/EvidenceExportLab.csproj') -- --verify
if ($LASTEXITCODE -ne 0) { throw 'Learner starter verification FAILED. Review ZIP readability, entry count and contents.' }
