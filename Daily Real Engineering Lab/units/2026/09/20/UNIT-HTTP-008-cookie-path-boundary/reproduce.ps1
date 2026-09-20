$ErrorActionPreference = 'Stop'
$out = dotnet run --project ./starter/CookiePathLab.csproj 2>&1 | Out-String
$out
if ($LASTEXITCODE -ne 0) { throw 'Starter failed to run.' }
if ($out -notmatch 'GET /admin/orders \| cookie-sent=True \| status=200') { throw 'Expected authenticated admin route was not observed.' }
if ($out -notmatch 'GET /reports/daily \| cookie-sent=False \| status=401') { throw 'Intended cross-route authentication symptom was not reproduced.' }
Write-Host 'REPRODUCED: login state is present on one protected route but absent on the sibling route.'
