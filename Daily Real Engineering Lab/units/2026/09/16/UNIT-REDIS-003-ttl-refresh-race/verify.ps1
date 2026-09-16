$ErrorActionPreference = 'Stop'
$output = dotnet run --project (Join-Path $PSScriptRoot 'starter/RedisRaceLab.csproj') 2>&1
$output | ForEach-Object { Write-Host $_ }
$lines = @($output | ForEach-Object { $_.ToString() })
$final = $lines | Where-Object { $_ -match '^FINAL ' } | Select-Object -Last 1
if ($final -match 'value=v2' -and $final -match 'expiresAt=10') {
    Write-Host 'VERIFIED: the refreshed state survives the stale expiry decision.'
    exit 0
}
throw 'Session refresh invariant is not preserved yet.'