$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/ObservabilityLab.csproj"
$templateLine = $output | Where-Object { $_ -like 'TEMPLATES=*' }
if (-not $templateLine) { throw 'Missing TEMPLATES evidence.' }
$count = [int]($templateLine -replace 'TEMPLATES=', '')
if ($count -lt 10) { throw "Expected high template cardinality in starter, got $count." }
Write-Host "Reproduced: template cardinality=$count"
