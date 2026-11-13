$ErrorActionPreference='Stop'
$output = & dotnet run --project (Join-Path $PSScriptRoot 'starter/ScannerGate.csproj') 2>&1
$output | ForEach-Object { Write-Output $_ }
if ($LASTEXITCODE -ne 0) { throw 'Learner code failed' }
$text = $output -join "`n"
if ($text -notmatch 'MAX_ACTIVE=1') { throw 'Invariant not restored' }
if ($text -match 'B: entered') { throw 'Cancelled request entered critical section' }
if ($text -notmatch 'B: cancelled') { throw 'Cancellation path not exercised' }
Write-Output 'VERIFIED'
