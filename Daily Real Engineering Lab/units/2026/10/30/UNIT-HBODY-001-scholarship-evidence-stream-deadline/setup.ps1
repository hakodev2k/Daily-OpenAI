$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "harness/LabHarness.csproj"
dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet build $project --no-restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
