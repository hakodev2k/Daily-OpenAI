$ErrorActionPreference='Stop'
$out = dotnet run --project (Join-Path $PSScriptRoot 'starter/PgRecoveryLab.csproj') 2>&1
$out | ForEach-Object { Write-Host $_ }
$text = $out -join "`n"
if ($text -match 'COMMIT OK' -and $text -match 'PERSISTED A,B' -and $text -notmatch 'current transaction is aborted') {
  Write-Host 'VERIFIED: invalid row is isolated and valid rows remain committable.'
  exit 0
}
throw 'Not fixed: transaction recovery contract is not satisfied.'