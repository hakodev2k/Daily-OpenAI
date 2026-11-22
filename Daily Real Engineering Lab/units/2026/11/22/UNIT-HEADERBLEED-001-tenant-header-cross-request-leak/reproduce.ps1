$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/Lab.csproj')
if ($LASTEXITCODE -eq 0) { throw 'Expected starter failure was not reproduced' }
Write-Host 'Reproduced tenant isolation failure'
