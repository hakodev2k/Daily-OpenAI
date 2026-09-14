$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot 'starter/MetricSeriesLab.csproj'
$output = & dotnet run --project $project 2>&1
if ($LASTEXITCODE -ne 0) { throw "Starter failed to run.`n$output" }

$output | ForEach-Object { Write-Host $_ }

$measurementsLine = $output | Where-Object { $_ -match '^Measurements=(\d+)$' } | Select-Object -Last 1
$seriesLine = $output | Where-Object { $_ -match '^DistinctSeries=(\d+)$' } | Select-Object -Last 1
if (-not $measurementsLine -or -not $seriesLine) { throw 'Expected evidence was not emitted.' }

$measurements = [int]($measurementsLine -replace '^Measurements=', '')
$series = [int]($seriesLine -replace '^DistinctSeries=', '')

if ($measurements -ne 500) { throw "Expected 500 measurements, got $measurements." }
if ($series -lt 100) { throw "Symptom was not reproduced: expected high series count, got $series." }

Write-Host 'REPRODUCE_PASS: high metric-series count confirmed.'
