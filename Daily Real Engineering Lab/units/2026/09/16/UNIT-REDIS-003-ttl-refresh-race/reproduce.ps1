$ErrorActionPreference = 'Stop'
$output = dotnet run --project (Join-Path $PSScriptRoot 'starter/RedisRaceLab.csproj') 2>&1
$output | ForEach-Object { Write-Host $_ }
if (@($output | ForEach-Object { $_.ToString() }) -match '^FINAL missing version=2$') {
    Write-Host 'REPRODUCED: refreshed session is lost under the controlled interleaving.'
    exit 0
}
throw 'Starter no longer reproduces the intended symptom.'