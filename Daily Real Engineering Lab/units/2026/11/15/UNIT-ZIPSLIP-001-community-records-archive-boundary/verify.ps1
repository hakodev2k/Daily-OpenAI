$ErrorActionPreference = 'Stop'
$root = Join-Path $PSScriptRoot 'sandbox'
$project = Join-Path $PSScriptRoot 'starter/ArchiveIntake.csproj'
if (Test-Path $root) { Remove-Item $root -Recurse -Force }
dotnet run --project $project -- --fixture $root
if ($LASTEXITCODE -ne 0) { throw 'Fixture failed' }
dotnet run --project $project -- --extract $root
if ($LASTEXITCODE -ne 0) { throw 'Extraction failed' }
if (Test-Path (Join-Path $root 'outside.txt')) { throw 'FAIL: untrusted entry escaped extracted/' }
$safe = Join-Path $root 'extracted/records/2026/report.txt'
if (!(Test-Path $safe)) { throw 'FAIL: legitimate file missing' }
if ((Get-Content $safe -Raw).Trim() -ne 'Approved community record') { throw 'FAIL: valid file corrupted' }
Write-Host 'PASS: valid record extracted, no outside write'
