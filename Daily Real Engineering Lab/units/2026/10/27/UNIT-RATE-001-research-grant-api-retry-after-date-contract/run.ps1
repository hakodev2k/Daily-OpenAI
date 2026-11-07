$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/RetryWindowLab.csproj')
exit $LASTEXITCODE
