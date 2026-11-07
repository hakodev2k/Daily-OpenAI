$ErrorActionPreference='Stop'
dotnet build ./starter/CmsCacheLab.csproj
$out=dotnet run --project ./starter -- verify 2>&1 | Out-String
$out
if($LASTEXITCODE -ne 0 -or $out -notmatch 'VERIFY_PASS'){throw 'Learner-editable starter still serves stale published content.'}
Write-Host 'VERIFICATION_PASS'