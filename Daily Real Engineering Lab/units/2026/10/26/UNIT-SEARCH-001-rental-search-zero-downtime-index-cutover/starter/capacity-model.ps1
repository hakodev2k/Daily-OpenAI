$ErrorActionPreference = 'Stop'
$csv = Join-Path (Join-Path $PSScriptRoot '..') 'evidence/capacity.csv'
$rows = Import-Csv -LiteralPath $csv
foreach ($row in $rows) {
    $documents = [double]::Parse($row.documents, [System.Globalization.CultureInfo]::InvariantCulture)
    $backfillRate = [double]::Parse($row.backfillDocsPerSec, [System.Globalization.CultureInfo]::InvariantCulture)
    $sourceRate = [double]::Parse($row.sourceEventsPerSec, [System.Globalization.CultureInfo]::InvariantCulture)
    $replayRate = [double]::Parse($row.replayEventsPerSec, [System.Globalization.CultureInfo]::InvariantCulture)
    $retention = [double]::Parse($row.cdcRetentionHours, [System.Globalization.CultureInfo]::InvariantCulture)
    if ($backfillRate -le 0) { throw 'backfillDocsPerSec must be positive' }
    $backfillHours = $documents / $backfillRate / 3600.0
    $backlog = $sourceRate * $documents / $backfillRate
    $netReplay = $replayRate - $sourceRate
    if ($netReplay -le 0) {
        Write-Output ("{0}: backfill={1:N2}h backlog={2:N0} events; NO FINITE CATCH-UP (net={3:N0}/s)" -f $row.scenario, $backfillHours, $backlog, $netReplay)
        continue
    }
    $catchupHours = $backlog / $netReplay / 3600.0
    $totalHours = $backfillHours + $catchupHours
    $retentionSafe = $totalHours -lt $retention
    Write-Output ("{0}: backfill={1:N2}h backlog={2:N0} events catchup={3:N2}h total={4:N2}h retentionSafe={5}" -f $row.scenario, $backfillHours, $backlog, $catchupHours, $totalHours, $retentionSafe)
}
