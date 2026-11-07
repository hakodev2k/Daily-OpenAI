$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/RosterLab.csproj'
& dotnet run --project $project -- verify
exit $LASTEXITCODE
