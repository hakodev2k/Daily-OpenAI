$ErrorActionPreference='Stop'
$out = dotnet run --project ./starter/WorkerLab.csproj
$out | Write-Host
if ($out -notmatch 'cycle=1; database=v1; observed=v1') { throw 'Cycle 1 sai behavior.' }
if ($out -notmatch 'cycle=2; database=v2; observed=v2') { throw 'Cycle 2 chưa quan sát state mới.' }
$ids = @($out | ForEach-Object { if ($_ -match 'session=(\d+)') { $Matches[1] } })
if ($ids.Count -lt 2 -or $ids[0] -eq $ids[1]) { throw 'Processing cycles chưa có dependency lifecycle độc lập.' }
Write-Host 'PASS: behavior đúng và lifecycle được tách theo processing cycle.'