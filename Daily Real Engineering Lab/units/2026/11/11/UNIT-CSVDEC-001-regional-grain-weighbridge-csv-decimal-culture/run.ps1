$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Weighbridge.csproj'
dotnet run --project $project -- en-US
exit $LASTEXITCODE
