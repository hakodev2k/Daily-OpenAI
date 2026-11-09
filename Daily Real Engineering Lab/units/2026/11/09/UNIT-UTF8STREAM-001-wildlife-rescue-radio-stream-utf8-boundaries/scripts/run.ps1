$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/RadioGateway.csproj'
dotnet run --project $project -- --run
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
