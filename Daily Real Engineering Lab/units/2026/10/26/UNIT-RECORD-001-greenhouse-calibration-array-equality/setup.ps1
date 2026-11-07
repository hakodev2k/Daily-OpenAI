$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "harness/GreenhouseLab.Harness.csproj"
dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet build $project --configuration Release --no-restore
exit $LASTEXITCODE
