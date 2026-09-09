$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot 'starter/DocumentPreview/DocumentPreview.csproj'
dotnet run --project $project
exit $LASTEXITCODE
