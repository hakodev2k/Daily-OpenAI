$ErrorActionPreference = 'Stop'

$output = dotnet run --project "$PSScriptRoot/starter/Lab/Lab.csproj" -- --requests 2000
$output | ForEach-Object { Write-Host $_ }

$measurements = [int](($output | Where-Object { $_ -like 'measurements=*' }) -replace 'measurements=', '')
$series = [int](($output | Where-Object { $_ -like 'series_count=*' }) -replace 'series_count=', '')
$dimensions = (($output | Where-Object { $_ -like 'dimension_names=*' }) -replace 'dimension_names=', '')

if ($measurements -ne 2000) { throw "Expected 2000 measurements, got $measurements." }
if ($series -lt 1000) { throw "Starter symptom not reproduced: expected at least 1000 series, got $series." }
if ($dimensions -notmatch 'user_id') { throw "Starter symptom not reproduced: expected user_id among metric dimensions." }

Write-Host "REPRODUCED: traffic is stable but metric series scale with user-level data."
