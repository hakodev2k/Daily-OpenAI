$ErrorActionPreference = 'Stop'
$project = Join-Path (Split-Path -Parent $PSScriptRoot) 'starter/TransferLab.csproj'
dotnet run --project $project -c Release -- --mode smoke
exit $LASTEXITCODE
