$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/SignalDispatch.csproj')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
