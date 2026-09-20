$ErrorActionPreference='Stop'
dotnet build ./starter/CmsCacheLab.csproj
$out=dotnet run --project ./starter -- reproduce 2>&1 | Out-String
$out
if($LASTEXITCODE -ne 0 -or $out -notmatch 'SYMPTOM_REPRODUCED'){throw 'Expected stale-content symptom was not reproduced.'}
Write-Host 'REPRODUCE_PASS'