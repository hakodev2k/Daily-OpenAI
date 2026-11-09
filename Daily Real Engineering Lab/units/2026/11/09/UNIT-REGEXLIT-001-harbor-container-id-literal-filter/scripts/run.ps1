$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/HarborInventory.csproj'
dotnet run --project $project
exit $LASTEXITCODE
