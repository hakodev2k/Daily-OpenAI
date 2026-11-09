$ErrorActionPreference = 'Stop'
$events = Import-Csv (Join-Path $PSScriptRoot 'evidence/incident-timeline.csv')
$accepted = @($events | Where-Object event -eq 'ACCEPTED')
$confirmed = @($events | Where-Object event -eq 'RELAY_ON')
Write-Output "Replay: dashboard claims ACTIVE_COUNT=240 at 10:00:03"
Write-Output "Gateway accepted events in sample: $($accepted.Count)"
Write-Output "Observed relay-on events in sample: $($confirmed.Count)"
Write-Output "Replayed events:"
$events | Format-Table time,site,event,command,version,detail -AutoSize | Out-String | Write-Output
Write-Output 'Investigation: Does broker delivery or gateway acceptance prove physical activation?'
