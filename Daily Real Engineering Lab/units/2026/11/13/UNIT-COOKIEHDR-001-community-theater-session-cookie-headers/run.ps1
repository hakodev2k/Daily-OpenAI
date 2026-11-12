$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/CookieLab.csproj')
exit $LASTEXITCODE
