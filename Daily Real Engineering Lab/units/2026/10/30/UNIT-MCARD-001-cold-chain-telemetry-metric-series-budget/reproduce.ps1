$ErrorActionPreference = 'Stop'
$result = & (Join-Path $PSScriptRoot 'scripts/measure.ps1') -PolicyPath (Join-Path $PSScriptRoot 'starter/metric-policy.json') | ConvertFrom-Json
if ($result.events -ne 360 -or $result.operationalGroups -ne 18) { throw 'Fixture contract mismatch.' }
if ($result.activeSeries -le $result.localSeriesBudget) {
    throw "Expected original symptom absent: $($result.activeSeries) series."
}
Write-Output "REPRODUCED: $($result.activeSeries) active series > $($result.localSeriesBudget) budget; $($result.events) events."
