$ErrorActionPreference='Stop'
$out = dotnet run --project ./starter/Starter.csproj 2>&1 | Out-String
$out
if ($out -match 'Materialized=5000') { throw 'Verification failed: starter still materializes the full customer set.' }
if ($out -notmatch 'Returned=20') { throw 'Verification failed: expected 20 result rows.' }
Write-Host 'Verification passed: learner path returns 20 rows without the original full-materialization symptom.'