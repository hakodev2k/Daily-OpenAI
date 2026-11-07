$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/SettlementOrderLab.csproj'
& dotnet --version
if ($LASTEXITCODE -ne 0) { throw 'Yêu cầu .NET SDK 10.0.100 hoặc feature band tương thích.' }
& dotnet restore $project
if ($LASTEXITCODE -ne 0) { throw 'Restore thất bại.' }
Write-Host 'Setup hoàn tất.'
