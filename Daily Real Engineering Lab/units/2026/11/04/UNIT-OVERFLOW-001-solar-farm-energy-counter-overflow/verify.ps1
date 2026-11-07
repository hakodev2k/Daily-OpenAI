$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/SolarLedger.csproj'
$output = @(dotnet run --project $project)
if ($LASTEXITCODE -ne 0) { throw 'Learner code failed to execute' }
$output | ForEach-Object { Write-Output $_ }
$actual = @{}; $expected = @{}
foreach ($line in $output) {
  if ($line -match '^ACTUAL (\w+) (-?\d+)$') { $actual[$Matches[1]] = [long]::Parse($Matches[2]) }
  if ($line -match '^EXPECTED (\w+) (-?\d+)$') { $expected[$Matches[1]] = [long]::Parse($Matches[2]) }
}
foreach ($key in @('small','peak','boundary')) {
  if (-not $actual.ContainsKey($key) -or -not $expected.ContainsKey($key)) { throw "Missing scenario: $key" }
  if ($actual[$key] -ne $expected[$key]) { throw "FAILED $key : actual=$($actual[$key]) expected=$($expected[$key])" }
}
if ($actual['peak'] -ne 3300000000L -or $actual['boundary'] -ne 2147483648L) { throw 'Incorrect boundary oracle' }
Write-Output 'PASS: learner code preserves small, peak and boundary aggregates.'
