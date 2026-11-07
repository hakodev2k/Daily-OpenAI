$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path

dotnet run --project (Join-Path $root 'starter/RateCounterLab.csproj')
exit $LASTEXITCODE
