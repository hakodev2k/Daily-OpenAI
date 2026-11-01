$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../checks/Checks.csproj'
dotnet run --project $project -- verify
exit $LASTEXITCODE
