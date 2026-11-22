$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/Lab.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Isolation contract still fails' }
Write-Host 'Verified tenant isolation'
