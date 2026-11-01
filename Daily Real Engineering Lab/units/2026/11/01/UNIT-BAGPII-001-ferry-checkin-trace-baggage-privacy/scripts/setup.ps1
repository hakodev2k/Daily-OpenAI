$ErrorActionPreference = 'Stop'
& dotnet --version
if ($LASTEXITCODE -ne 0) { throw 'Cần .NET SDK 8.x.' }
$project = Join-Path $PSScriptRoot '../harness/TraceHarness.csproj'
& dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
