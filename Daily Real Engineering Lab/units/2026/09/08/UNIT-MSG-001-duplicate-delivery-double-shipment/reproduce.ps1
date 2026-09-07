$ErrorActionPreference = "Stop"
$output = & (Join-Path $PSScriptRoot "run.ps1")
Write-Host $output
$match = $output | Select-String '^ReservationsForOrder=(\d+)$'
if (-not $match) { Write-Error "Expected diagnostic output not found."; exit 1 }
$count = [int]$match.Matches.Groups[1].Value
if ($count -le 1) { Write-Error "Expected duplicate side effect was not reproduced."; exit 1 }
Write-Host "PASS: duplicate delivery reproduced a duplicate shipment reservation."
