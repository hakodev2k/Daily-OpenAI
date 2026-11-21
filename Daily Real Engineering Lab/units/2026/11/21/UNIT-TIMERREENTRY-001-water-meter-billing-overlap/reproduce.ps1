$out = & dotnet run --project (Join-Path $PSScriptRoot 'starter/TimerLab.csproj') -c Release 2>&1
$code=$LASTEXITCODE
$out | ForEach-Object { Write-Output $_ }
$s=$out | Out-String
if($code -eq 2 -and $s -match 'MAX_ACTIVE=2' -and $s -match 'POSTED=2' -and $s -match 'CYCLES=2'){Write-Output 'REPRODUCED';exit 0}
Write-Error "Expected failure exit 2 and two posts, actual exit $code"
exit 1
