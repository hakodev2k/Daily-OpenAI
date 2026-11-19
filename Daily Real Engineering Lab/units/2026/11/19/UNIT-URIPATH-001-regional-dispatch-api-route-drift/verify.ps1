$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/RegionalDispatch.csproj') -- verify
if ($LASTEXITCODE -ne 0) { throw 'Learner starter chưa đạt contract' }
