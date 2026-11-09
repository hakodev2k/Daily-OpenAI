$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/HarborInventory.csproj'
dotnet --version
dotnet restore $project
exit $LASTEXITCODE
