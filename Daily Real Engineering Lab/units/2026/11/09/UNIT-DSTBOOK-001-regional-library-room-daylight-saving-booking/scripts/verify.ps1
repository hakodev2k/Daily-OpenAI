$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/DstBooking.csproj'
function Check([string]$local, [string]$offset, [string]$status, [string]$utc, [string]$errorCode) {
  $out = & dotnet run --project $project -- $local 'America/New_York' $offset
  if ($LASTEXITCODE -ne 0) { throw "process failed: $local $offset" }
  $actual = $out | Select-Object -Last 1 | ConvertFrom-Json
  if ($actual.Status -ne $status -or $actual.Utc -ne $utc -or $actual.Error -ne $errorCode) {
    throw "FAIL $local $offset: $($actual | ConvertTo-Json -Compress)"
  }
  Write-Host "PASS $local $offset"
}
Check '2026-11-02 10:00' '-' 'CONFIRMED' '2026-11-02T15:00:00.0000000Z' $null
Check '2026-03-08 02:30' '-' 'REJECTED' $null 'nonexistent-local-time'
Check '2026-11-01 01:30' '-' 'REJECTED' $null 'offset-required'
Check '2026-11-01 01:30' '-04:00' 'CONFIRMED' '2026-11-01T05:30:00.0000000Z' $null
Check '2026-11-01 01:30' '-05:00' 'CONFIRMED' '2026-11-01T06:30:00.0000000Z' $null
Check '2026-11-01 01:30' '-06:00' 'REJECTED' $null 'offset-not-valid-for-zone'
Check '2026-11-02 10:00' '-04:00' 'REJECTED' $null 'offset-not-valid-for-zone'
Check 'bad' '-' 'REJECTED' $null 'invalid-format'
Write-Host 'All learner-workspace contracts passed'
