$ErrorActionPreference = 'Stop'
$out = dotnet run --project ./starter/Starter.csproj
$out | Write-Output
$text = $out -join "`n"
if ($text -notmatch 'records=20') { throw 'Correctness regression: expected 20 tenant records.' }
if ($text -notmatch 'partitionsTouched=1') { throw 'Engineering property not fixed: expected one logical partition.' }
if ($text -notmatch 'scanned=20') { throw 'Expected scan scope is not bounded to tenant partition.' }
Write-Host 'Verified: learner path preserves records and bounds query work to one partition.'