$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'checks/Settlement.Checks/Settlement.Checks.csproj'
dotnet run --project $project -- verify
exit $LASTEXITCODE
