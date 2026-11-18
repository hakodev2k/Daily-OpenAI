$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Verification.csproj'
dotnet run --project $project
exit $LASTEXITCODE
