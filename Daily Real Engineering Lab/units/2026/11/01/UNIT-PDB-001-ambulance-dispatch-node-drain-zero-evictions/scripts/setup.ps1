$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/PdbLab.csproj'
& dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
