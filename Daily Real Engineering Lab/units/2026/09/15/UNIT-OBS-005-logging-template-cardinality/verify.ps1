$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/ObservabilityLab.csproj"
$eventsLine = $output | Where-Object { $_ -like 'EVENTS=*' }
$templateLine = $output | Where-Object { $_ -like 'TEMPLATES=*' }
if (-not $eventsLine -or -not $templateLine) { throw 'Missing verification evidence.' }
$events = [int]($eventsLine -replace 'EVENTS=', '')
$templates = [int]($templateLine -replace 'TEMPLATES=', '')
if ($events -ne 20) { throw "Functional regression: expected 20 events, got $events." }
if ($templates -gt 2) { throw "Telemetry contract not fixed: expected <=2 stable templates, got $templates." }
Write-Host "Verified: events=$events templates=$templates"
