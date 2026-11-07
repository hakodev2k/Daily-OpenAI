$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot 'starter/MembershipScanLab.csproj'
$output = & dotnet run --project $project 2>&1
if ($LASTEXITCODE -ne 0) { throw "Starter failed to run.`n$output" }

$output | ForEach-Object { Write-Host $_ }

$matchedLine = $output | Where-Object { $_ -match '^Matched=(\d+)$' } | Select-Object -Last 1
$comparisonLine = $output | Where-Object { $_ -match '^Comparisons=(\d+)$' } | Select-Object -Last 1
if (-not $matchedLine -or -not $comparisonLine) { throw 'Expected evidence was not emitted.' }

$matched = [int]($matchedLine -replace '^Matched=', '')
$comparisons = [long]($comparisonLine -replace '^Comparisons=', '')

if ($matched -ne 1000) { throw "Unexpected business result: expected 1000, got $matched." }
if ($comparisons -lt 500000) { throw "Symptom was not reproduced: expected heavy comparison amplification, got $comparisons." }

Write-Host 'REPRODUCE_PASS: correct output with excessive membership comparisons confirmed.'
