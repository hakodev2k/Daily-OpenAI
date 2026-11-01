$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/PdbLab.csproj'
& dotnet run --project $project -- verify
exit $LASTEXITCODE
