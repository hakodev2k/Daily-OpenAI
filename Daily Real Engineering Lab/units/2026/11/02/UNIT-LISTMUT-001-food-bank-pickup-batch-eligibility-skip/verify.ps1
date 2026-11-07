$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'checks/Checks.csproj'
dotnet run --project $project -- verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
