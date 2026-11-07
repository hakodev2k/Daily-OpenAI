$ErrorActionPreference = "Stop"
$output = & (Join-Path $PSScriptRoot "run.ps1")
Write-Host $output
$resMatch = $output | Select-String '^ReservationsForOrder=(\d+)$'
$deliveryMatch = $output | Select-String '^DeliveryCount=(\d+)$'
if (-not $resMatch -or -not $deliveryMatch) { Write-Error "Expected diagnostic output not found."; exit 1 }
$reservations = [int]$resMatch.Matches.Groups[1].Value
$deliveries = [int]$deliveryMatch.Matches.Groups[1].Value
if ($deliveries -ne 2) { Write-Error "Expected two deliveries in the deterministic scenario."; exit 1 }
if ($reservations -ne 1) { Write-Error "FAIL: expected exactly one reservation for the order, got $reservations."; exit 1 }
Write-Host "PASS: duplicate delivery no longer duplicates the shipment side effect."
