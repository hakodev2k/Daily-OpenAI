$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ExhibitLookup.csproj'
$output = (& dotnet run --project $project 2>&1 | Out-String)
if ($LASTEXITCODE -ne 0) { throw "Starter failed to run: $output" }
if ($output -notmatch 'count=1' -or $output -notmatch 'RESULT found=NO' -or $output -notmatch 'LOOKUP fresh-key=False') { throw "Expected original symptom not reproduced: $output" }
Write-Host 'PASS: original lookup failure reproduced (not a fix verification).'
