$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/CohortLab.csproj') -- report
exit $LASTEXITCODE
