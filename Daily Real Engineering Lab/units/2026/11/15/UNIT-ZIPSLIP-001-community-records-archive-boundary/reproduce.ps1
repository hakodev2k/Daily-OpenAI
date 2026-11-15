$ErrorActionPreference = 'Stop'
$root = Join-Path $PSScriptRoot 'sandbox'
$project = Join-Path $PSScriptRoot 'starter/ArchiveIntake.csproj'
if (Test-Path $root) { Remove-Item $root -Recurse -Force }
dotnet run --project $project -- --fixture $root
if ($LASTEXITCODE -ne 0) { throw 'Fixture failed' }
dotnet run --project $project -- --extract $root
if ($LASTEXITCODE -ne 0) { throw 'Starter failed' }
$outside = Join-Path $root 'outside.txt'
if (!(Test-Path $outside)) { throw 'Expected baseline symptom missing: outside.txt not written' }
Write-Host 'BASELINE REPRODUCED: entry wrote outside extracted/ directory'
