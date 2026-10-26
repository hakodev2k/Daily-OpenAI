$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/TrialImport/TrialImport.csproj'
& dotnet restore $project
exit $LASTEXITCODE
