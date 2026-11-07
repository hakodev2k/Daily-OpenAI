$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/CohortLab.csproj') -- verify
exit $LASTEXITCODE
