$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/TreeInventory.csproj') -- --reproduce
if ($LASTEXITCODE -ne 0) { throw 'Không reproduce được triệu chứng; kiểm tra trạng thái starter.' }
Write-Host 'Reproduction confirmed.'
