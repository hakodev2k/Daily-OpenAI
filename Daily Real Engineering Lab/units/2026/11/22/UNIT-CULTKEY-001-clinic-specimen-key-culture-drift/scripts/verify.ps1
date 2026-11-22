$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
dotnet run --project (Join-Path $root 'starter/SpecimenLab.csproj') -- --verify
if ($LASTEXITCODE -ne 0) { throw 'Verify thất bại: sửa starter rồi chạy lại.' }
