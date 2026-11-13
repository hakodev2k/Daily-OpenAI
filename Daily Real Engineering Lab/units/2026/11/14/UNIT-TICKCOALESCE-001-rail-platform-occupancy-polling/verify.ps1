$ErrorActionPreference='Stop'
$unit=Split-Path -Parent $MyInvocation.MyCommand.Path
$lines = & dotnet run --project (Join-Path $unit 'starter/RailPoller.csproj') --no-launch-profile 2>&1
if ($LASTEXITCODE -ne 0) { throw "Learner starter failed: $lines" }
$lines | ForEach-Object { Write-Host $_ }
$summary = ($lines | Select-String 'SUMMARY observedSlots=(\d+) missedSlots=(\d+) expectedSlots=(\d+)' | Select-Object -Last 1)
if (-not $summary) { throw 'Missing reconciled summary; fix starter/Program.cs and expose observedSlots/missedSlots/expectedSlots' }
$o=[int]$summary.Matches[0].Groups[1].Value; $m=[int]$summary.Matches[0].Groups[2].Value; $e=[int]$summary.Matches[0].Groups[3].Value
if ($o -lt 1 -or $m -lt 1 -or $e -lt 1 -or ($o+$m) -ne $e) { throw "Invariant failed: observed=$o missed=$m expected=$e" }
if ($lines -match 'FAKE_SAMPLE') { throw 'Never synthesize camera observations for missed slots' }
Write-Host 'PASS: slot accounting invariant and no synthetic backfill marker'
