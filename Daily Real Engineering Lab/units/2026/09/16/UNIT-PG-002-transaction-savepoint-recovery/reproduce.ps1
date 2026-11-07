$ErrorActionPreference='Stop'
$out = dotnet run --project (Join-Path $PSScriptRoot 'starter/PgRecoveryLab.csproj') 2>&1
$out | ForEach-Object { Write-Host $_ }
$text = $out -join "`n"
if ($text -match 'SKIPPED DUPLICATE' -and $text -match 'current transaction is aborted|COMMIT FAILED') {
  Write-Host 'REPRODUCED: caught row failure did not restore a usable transaction state.'
  exit 0
}
throw 'Starter did not reproduce the expected transaction-recovery symptom.'