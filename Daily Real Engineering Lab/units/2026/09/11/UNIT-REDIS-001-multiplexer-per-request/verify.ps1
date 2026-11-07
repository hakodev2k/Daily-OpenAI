$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Lab.csproj'
$output = dotnet run --project $project --configuration Release 2>&1
$output | ForEach-Object { Write-Host $_ }

function Read-Metric([string]$name) {
    $line = $output | Where-Object { $_ -match "^$name=" } | Select-Object -Last 1
    if (-not $line) { throw "Thiếu metric: $name" }
    return [int](($line -split '=')[1])
}

$results = Read-Metric 'results'
$distinct = Read-Metric 'distinctPrices'
$clients = Read-Metric 'clientsCreated'
$handshakes = Read-Metric 'handshakes'

if ($results -ne 24) { throw "FAIL: expected 24 results, got $results" }
if ($distinct -ne 1) { throw "FAIL: inconsistent business result, distinctPrices=$distinct" }
if ($clients -gt 2) { throw "FAIL: client lifecycle vẫn churn, clientsCreated=$clients" }
if ($handshakes -gt 2) { throw "FAIL: handshake vẫn bị lặp, handshakes=$handshakes" }

Write-Host 'PASS: correctness giữ nguyên và connection-oriented client được reuse.'
