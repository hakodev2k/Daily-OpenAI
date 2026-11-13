$ErrorActionPreference='Stop'
$unit=Split-Path -Parent $MyInvocation.MyCommand.Path
$lines = & dotnet run --project (Join-Path $unit 'starter/RailPoller.csproj') --no-launch-profile 2>&1
if ($LASTEXITCODE -ne 0) { throw "Starter failed: $lines" }
$lines | ForEach-Object { Write-Host $_ }
$summary = ($lines | Select-String 'SUMMARY polls=(\d+) expectedSlots=(\d+)' | Select-Object -Last 1)
if (-not $summary) { throw 'Missing summary' }
$p=[int]$summary.Matches[0].Groups[1].Value; $e=[int]$summary.Matches[0].Groups[2].Value
if ($e -le $p) { throw "Symptom not reproduced: expectedSlots=$e polls=$p" }
Write-Host "REPRODUCED: elapsed nominal slots $e > polls $p"
