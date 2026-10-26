$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/TrialImport/TrialImport.csproj'
& dotnet run --project $project -- demo
exit $LASTEXITCODE
