$ErrorActionPreference = 'Stop'
$project = Join-Path (Split-Path -Parent $PSScriptRoot) 'starter/TransferLab.csproj'
dotnet restore $project
exit $LASTEXITCODE
