$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/RegionalDispatch.csproj') -- reproduce
if ($LASTEXITCODE -ne 0) { throw 'Starter không còn tái hiện triệu chứng' }
