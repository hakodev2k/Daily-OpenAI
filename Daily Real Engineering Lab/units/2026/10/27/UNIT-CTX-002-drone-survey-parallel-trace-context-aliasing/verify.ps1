$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/TraceLab/TraceLab.csproj'
dotnet run --project $project -- verify
exit $LASTEXITCODE
