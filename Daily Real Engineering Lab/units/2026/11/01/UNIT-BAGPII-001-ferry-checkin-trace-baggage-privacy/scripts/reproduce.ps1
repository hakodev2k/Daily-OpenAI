$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../harness/TraceHarness.csproj'
& dotnet run --project $project -- reproduce
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
