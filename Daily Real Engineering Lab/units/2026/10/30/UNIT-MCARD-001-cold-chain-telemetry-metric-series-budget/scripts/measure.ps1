param(
    [Parameter(Mandatory = $true)]
    [string]$PolicyPath
)
$ErrorActionPreference = 'Stop'
$policy = Get-Content -LiteralPath $PolicyPath -Raw | ConvertFrom-Json
[string[]]$labels = @($policy.labels)
$allowed = @('region', 'depot', 'sensor_id', 'outcome', 'request_id')
if ($labels.Count -eq 0) { throw 'Policy must declare at least one label.' }
if (($labels | Select-Object -Unique).Count -ne $labels.Count) { throw 'Duplicate label name.' }
foreach ($label in $labels) {
    if ($allowed -cnotcontains $label) { throw "Unknown label: $label" }
}
$series = @{}
$groups = @{}
$events = 0
foreach ($region in @('north', 'south')) {
    foreach ($depotIndex in 1..3) {
        foreach ($sensorIndex in 1..4) {
            foreach ($outcome in @('ok', 'warning', 'critical')) {
                foreach ($attempt in 1..5) {
                    $event = @{
                        region = $region
                        depot = "$region-depot-$depotIndex"
                        sensor_id = "$region-$depotIndex-sensor-$sensorIndex"
                        outcome = $outcome
                        request_id = "$region-$depotIndex-$sensorIndex-$outcome-$attempt"
                    }
                    $parts = foreach ($label in $labels) { "$label=$($event[$label])" }
                    $key = $parts -join '|'
                    $series[$key] = $true
                    $groupKey = "$($event.region)|$($event.depot)|$($event.outcome)"
                    $groups[$groupKey] = $true
                    $events++
                }
            }
        }
    }
}
[pscustomobject]@{
    metric = $policy.metric
    labels = $labels
    events = $events
    activeSeries = $series.Count
    operationalGroups = $groups.Count
    localSeriesBudget = 36
} | ConvertTo-Json -Compress -Depth 4
