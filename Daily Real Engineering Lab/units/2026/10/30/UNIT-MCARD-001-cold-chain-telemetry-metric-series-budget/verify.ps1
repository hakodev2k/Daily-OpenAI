$ErrorActionPreference = 'Stop'
$result = & (Join-Path $PSScriptRoot 'scripts/measure.ps1') -PolicyPath (Join-Path $PSScriptRoot 'starter/metric-policy.json') | ConvertFrom-Json
if ($result.events -ne 360 -or $result.operationalGroups -ne 18) { throw 'Fixture contract mismatch.' }
foreach ($required in @('region', 'depot', 'outcome')) {
    if (@($result.labels) -cnotcontains $required) { throw "Required operational dimension missing: $required" }
}
if ($result.activeSeries -gt $result.localSeriesBudget) {
    throw "FAIL: $($result.activeSeries) series exceeds $($result.localSeriesBudget)."
}
Write-Output "PASS: $($result.activeSeries) series <= $($result.localSeriesBudget), all required operational dimensions preserved."
