$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/PermitLab.csproj'
dotnet run --project $project -- reproduce
exit $LASTEXITCODE
