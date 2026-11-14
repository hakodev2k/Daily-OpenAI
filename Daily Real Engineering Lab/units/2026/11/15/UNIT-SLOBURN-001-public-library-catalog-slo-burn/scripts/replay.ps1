param([string]$InputFile = (Join-Path $PSScriptRoot '../evidence/windows.csv'))
$ErrorActionPreference = 'Stop'
$rows = Import-Csv $InputFile
foreach ($r in $rows) {
    $n = [double]::Parse($r.requests, [Globalization.CultureInfo]::InvariantCulture)
    $e = [double]::Parse($r.errors, [Globalization.CultureInfo]::InvariantCulture)
    if ($n -le 0 -or $e -lt 0 -or $e -gt $n) { throw "Invalid row: $($r.incident) $($r.window)" }
    [pscustomobject]@{
        Incident = $r.incident
        Window = $r.window
        Requests = $n
        Errors = $e
        ErrorPercent = [math]::Round(100 * $e / $n, 3)
        BurnRate = [math]::Round(($e / $n) / 0.001, 2)
        OldPage = (($e / $n) -gt 0.02 -and $r.window -eq '5m')
    }
} | Format-Table -AutoSize
