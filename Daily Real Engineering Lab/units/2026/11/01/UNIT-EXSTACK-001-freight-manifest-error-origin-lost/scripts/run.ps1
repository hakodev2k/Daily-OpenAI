$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../checks/Verifier.csproj'
dotnet run --project $project --configuration Release -- demo
exit $LASTEXITCODE
