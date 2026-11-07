$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Lab.csproj'
dotnet run --project $project -- @args
exit $LASTEXITCODE
