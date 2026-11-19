$ErrorActionPreference = 'Stop'
$output = & dotnet run --project (Join-Path $PSScriptRoot 'starter/SignalDispatch.csproj') -- --verify 2>&1
$code = $LASTEXITCODE
$output | ForEach-Object { Write-Host $_ }
if ($code -eq 1 -and (($output | Out-String) -match 'FAIL: command mapping differs')) { Write-Host 'PASS: starter symptom reproduced'; exit 0 }
Write-Error 'Expected starter failure was not reproduced (possibly already fixed, or build/runtime failure).'; exit 1
