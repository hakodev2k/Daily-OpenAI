$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "../starter/MetricCardinalityLab.csproj"
$output = dotnet run --project $project
$output | Out-Host
$line = $output | Where-Object { $_ -match '^series_count=' }
if (-not $line) { throw "VERIFY FAILED: series_count missing." }
$count = [int]($line -replace 'series_count=','')
if ($count -gt 10) { throw "VERIFY FAILED: metric cardinality remains too high ($count series)." }
Write-Host "VERIFY PASSED"
