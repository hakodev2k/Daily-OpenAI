$ErrorActionPreference='Stop'
$output = & dotnet run --project (Join-Path $PSScriptRoot 'starter/ScannerGate.csproj') 2>&1
$output | ForEach-Object { Write-Output $_ }
if ($LASTEXITCODE -ne 0) { throw 'Starter execution failed' }
if (($output -join "`n") -notmatch 'MAX_ACTIVE=2') { throw 'Reproduction failed: expected MAX_ACTIVE=2' }
Write-Output 'REPRODUCED'
