$ErrorActionPreference = 'Stop'

$output = dotnet run --project "$PSScriptRoot/starter/Lab/Lab.csproj" -- --requests 2000
$output | ForEach-Object { Write-Host $_ }

$measurements = [int](($output | Where-Object { $_ -like 'measurements=*' }) -replace 'measurements=', '')
$series = [int](($output | Where-Object { $_ -like 'series_count=*' }) -replace 'series_count=', '')
$dimensions = (($output | Where-Object { $_ -like 'dimension_names=*' }) -replace 'dimension_names=', '')

if ($measurements -ne 2000) { throw "Regression: expected 2000 measurements, got $measurements." }
if ($series -lt 1 -or $series -gt 20) { throw "Expected bounded metric series (1..20), got $series." }
if ($dimensions -ne 'region,status_code') { throw "Expected operational dimensions region,status_code; got '$dimensions'." }

Write-Host "VERIFIED: measurements are preserved and metric series remain bounded as user count grows."
