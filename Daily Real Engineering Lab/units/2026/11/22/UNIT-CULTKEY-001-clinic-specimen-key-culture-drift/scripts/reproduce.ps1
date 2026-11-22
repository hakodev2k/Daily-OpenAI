$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
dotnet run --project (Join-Path $root 'starter/SpecimenLab.csproj') -- --reproduce
if ($LASTEXITCODE -ne 0) { throw 'Reproduce không đạt: cần xác nhận starter và môi trường.' }
