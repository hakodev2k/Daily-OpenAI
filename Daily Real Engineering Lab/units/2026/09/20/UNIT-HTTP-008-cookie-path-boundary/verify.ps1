$ErrorActionPreference = 'Stop'
$out = dotnet run --project ./starter/CookiePathLab.csproj 2>&1 | Out-String
$out
if ($LASTEXITCODE -ne 0) { throw 'Learner starter failed to run.' }
if ($out -notmatch 'GET /admin/orders \| cookie-sent=True \| status=200') { throw 'Regression: admin route is no longer authenticated.' }
if ($out -notmatch 'GET /reports/daily \| cookie-sent=True \| status=200') { throw 'Fix incomplete: reports route still lacks authentication state.' }
Write-Host 'VERIFY_PASS: both protected routes receive the session cookie.'
