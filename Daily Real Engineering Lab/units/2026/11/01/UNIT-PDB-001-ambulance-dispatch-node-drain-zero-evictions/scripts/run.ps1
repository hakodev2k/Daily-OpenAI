$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/PdbLab.csproj'
& dotnet run --project $project -- inspect
exit $LASTEXITCODE
