$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'contract/Contract.csproj'
dotnet run --project $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
