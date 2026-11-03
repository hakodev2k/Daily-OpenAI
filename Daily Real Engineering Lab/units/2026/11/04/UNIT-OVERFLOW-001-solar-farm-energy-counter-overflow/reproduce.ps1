$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/SolarLedger.csproj'
$output = @(dotnet run --project $project)
if ($LASTEXITCODE -ne 0) { throw 'Starter failed to execute' }
$output | ForEach-Object { Write-Output $_ }
$actual = @{}; $expected = @{}
foreach ($line in $output) {
  if ($line -match '^ACTUAL (\w+) (-?\d+)$') { $actual[$Matches[1]] = [long]::Parse($Matches[2]) }
  if ($line -match '^EXPECTED (\w+) (-?\d+)$') { $expected[$Matches[1]] = [long]::Parse($Matches[2]) }
}
if ($actual.Count -ne 3 -or $expected.Count -ne 3) { throw 'Incomplete evidence' }
if ($actual['small'] -ne $expected['small']) { throw 'Baseline must pass' }
if ($actual['peak'] -eq $expected['peak'] -or $actual['boundary'] -eq $expected['boundary']) { throw 'Expected starter failure not observed' }
Write-Output 'REPRODUCED: large and boundary aggregates differ from independent oracle.'
