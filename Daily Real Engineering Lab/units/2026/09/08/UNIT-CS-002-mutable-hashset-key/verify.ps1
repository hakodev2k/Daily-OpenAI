$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"
$output = dotnet run --project $project
Write-Host $output

if ($output -notmatch "ContainsBeforeSecondAdd=True") { throw "FAIL: lookup is still unstable after normalization." }
if ($output -notmatch "SecondAddReturned=False") { throw "FAIL: logical duplicate was still accepted." }
if ($output -notmatch "FinalCount=1") { throw "FAIL: expected FinalCount=1." }

Write-Host "PASS: deduplication remains stable after normalization."
