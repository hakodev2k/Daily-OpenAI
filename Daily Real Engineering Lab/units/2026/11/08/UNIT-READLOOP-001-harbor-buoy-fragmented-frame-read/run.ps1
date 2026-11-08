$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/HarborFrame.csproj'
dotnet run --project $project -- fragmented
exit $LASTEXITCODE
