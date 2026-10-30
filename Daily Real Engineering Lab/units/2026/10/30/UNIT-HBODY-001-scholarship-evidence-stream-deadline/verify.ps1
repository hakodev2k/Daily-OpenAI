$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "harness/LabHarness.csproj"
dotnet run --project $project -- verify
exit $LASTEXITCODE
