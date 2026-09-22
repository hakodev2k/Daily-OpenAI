$ErrorActionPreference='Stop'
$out = dotnet run --project ./starter/Starter.csproj 2>&1 | Out-String
$out
if ($out -notmatch 'Materialized=5000; Returned=20') { throw 'Starter symptom was not reproduced.' }
Write-Host 'Reproduced: application materialized the full dataset for a 20-row response.'