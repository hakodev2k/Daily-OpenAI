$ErrorActionPreference = 'Stop'
$output = & dotnet run --project (Join-Path $PSScriptRoot 'starter/SignalDispatch.csproj') -- --verify 2>&1
$code = $LASTEXITCODE
$output | ForEach-Object { Write-Host $_ }
if ($code -ne 0 -or (($output | Out-String) -notmatch 'PASS: command mapping stable')) { Write-Error 'Verification failed'; exit 1 }
Write-Host 'PASS: learner starter meets cross-culture routing contract'
