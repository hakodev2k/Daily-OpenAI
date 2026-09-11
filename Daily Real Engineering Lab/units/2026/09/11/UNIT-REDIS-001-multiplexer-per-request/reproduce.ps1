$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Lab.csproj'
$output = dotnet run --project $project --configuration Release 2>&1
$output | ForEach-Object { Write-Host $_ }

$clientsLine = $output | Where-Object { $_ -match '^clientsCreated=' } | Select-Object -Last 1
$resultsLine = $output | Where-Object { $_ -match '^results=' } | Select-Object -Last 1
if (-not $clientsLine -or -not $resultsLine) { throw 'Không đọc được metrics từ starter.' }

$clients = [int](($clientsLine -split '=')[1])
$results = [int](($resultsLine -split '=')[1])
if ($results -ne 24) { throw "Functional result không đúng: results=$results" }
if ($clients -lt 20) { throw "Starter không còn thể hiện intended symptom: clientsCreated=$clients" }

Write-Host 'REPRODUCED: functional result đúng nhưng client/handshake setup bị lặp theo request.'
