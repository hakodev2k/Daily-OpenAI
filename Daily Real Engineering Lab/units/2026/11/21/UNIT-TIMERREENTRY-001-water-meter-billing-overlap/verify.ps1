$out = & dotnet run --project (Join-Path $PSScriptRoot 'starter/TimerLab.csproj') -c Release 2>&1
$code=$LASTEXITCODE
$out | ForEach-Object { Write-Output $_ }
$s=$out | Out-String
if($code -eq 0 -and $s -match 'MAX_ACTIVE=1' -and $s -match 'POSTED=1' -and $s -match 'CYCLES=1'){Write-Output 'VERIFIED';exit 0}
Write-Error "Starter did not satisfy single-flight contract, exit $code"
exit 1
