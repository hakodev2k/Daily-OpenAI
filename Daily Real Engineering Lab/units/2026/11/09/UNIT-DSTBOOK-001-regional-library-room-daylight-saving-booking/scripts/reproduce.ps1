$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/DstBooking.csproj'
function Invoke-Case([string]$local, [string]$offset) {
  $out = & dotnet run --project $project -- $local 'America/New_York' $offset
  return ($out | Select-Object -Last 1 | ConvertFrom-Json)
}
$normal = Invoke-Case '2026-11-02 10:00' '-'
$gap = Invoke-Case '2026-03-08 02:30' '-'
$overlap = Invoke-Case '2026-11-01 01:30' '-'
Write-Host "normal=$($normal | ConvertTo-Json -Compress)"
Write-Host "gap=$($gap | ConvertTo-Json -Compress)"
Write-Host "overlap=$($overlap | ConvertTo-Json -Compress)"
if ($normal.Status -ne 'CONFIRMED' -or $normal.Utc -ne '2026-11-02T15:00:00.0000000Z') { throw 'baseline fixture failed' }
if ($gap.Status -eq 'REJECTED' -and $overlap.Status -eq 'REJECTED') { throw 'Incident not reproduced' }
Write-Host 'REPRODUCED: at least one transition contract violated'
