$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../checks/Verifier.csproj'
dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet build $project --configuration Release --no-restore
exit $LASTEXITCODE
