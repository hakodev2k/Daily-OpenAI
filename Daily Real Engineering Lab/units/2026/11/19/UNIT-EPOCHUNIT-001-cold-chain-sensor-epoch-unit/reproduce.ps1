$ErrorActionPreference = 'Stop'
$out = dotnet run --project (Join-Path $PSScriptRoot 'starter/Telemetry.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Starter failed' }
$out | ForEach-Object { Write-Output $_ }
$s = $out -join "`n"
if ($s -notmatch 'A/A-17 status=ACCEPT' -or $s -notmatch 'B/B-22 status=STALE' -or $s -notmatch 'A/A-18 status=FUTURE') { throw 'Expected mismatch not reproduced' }
Write-Output 'REPRODUCED: B-22 is incorrectly stale'
