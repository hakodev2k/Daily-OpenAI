$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'src/LabHarness/LabHarness.csproj'
dotnet run --project $project -- info
if ($LASTEXITCODE -ne 0) { throw 'Harness diagnostic failed.' }
