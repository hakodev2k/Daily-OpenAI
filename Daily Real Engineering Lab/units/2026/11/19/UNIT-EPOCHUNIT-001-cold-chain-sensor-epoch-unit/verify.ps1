$ErrorActionPreference = 'Stop'
$out = dotnet run --project (Join-Path $PSScriptRoot 'starter/Telemetry.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Build/run failed' }
$out | ForEach-Object { Write-Output $_ }
$s = $out -join "`n"
if ($s -notmatch 'A/A-17 status=ACCEPT' -or $s -notmatch 'B/B-22 status=ACCEPT' -or $s -notmatch 'A/A-18 status=FUTURE') { throw 'FAIL: gateway classification regression' }
if (($out | Where-Object { $_ -match '^([AB])/' }).Count -ne 3) { throw 'FAIL: missing or duplicate packet' }
Write-Output 'PASS: all gateway timestamp classifications match contract'
