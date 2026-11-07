$ErrorActionPreference = 'Stop'
& dotnet run --project (Join-Path $PSScriptRoot 'starter/AdWindowLab.csproj') -- reproduce
exit $LASTEXITCODE
