$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ExhibitLookup.csproj'
$output = (& dotnet run --project $project 2>&1 | Out-String)
if ($LASTEXITCODE -ne 0) { throw "Learner starter failed: $output" }
if ($output -notmatch 'count=1' -or $output -notmatch 'LOOKUP fresh-key=True' -or $output -notmatch 'RESULT found=YES') { throw "Fix not verified: $output" }
Write-Host 'PASS: learner workspace lookup and cardinality invariants verified.'
