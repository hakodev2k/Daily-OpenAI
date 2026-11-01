$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/RosterLab.csproj'
& dotnet run --project $project -- report
exit $LASTEXITCODE
