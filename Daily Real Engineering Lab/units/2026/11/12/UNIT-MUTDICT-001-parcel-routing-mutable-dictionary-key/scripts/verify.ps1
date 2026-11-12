$root = Split-Path $PSScriptRoot -Parent
$output = & dotnet run --project (Join-Path $root 'starter/ParcelLab.csproj') 2>&1
$exitCode = $LASTEXITCODE
$output | ForEach-Object { Write-Output $_ }
$text = $output -join "`n"
if ($exitCode -eq 0 -and $text -match 'PASS: corrected route is searchable' -and $text -match 'AFTER count=1 found=True' -and $text -match 'LOOKUP found=True route=EAST') { Write-Output 'VERIFIED: learner starter passes behavior contract'; exit 0 }
Write-Error 'Verification failed: fix learner-editable starter, not solution.'
exit 1
