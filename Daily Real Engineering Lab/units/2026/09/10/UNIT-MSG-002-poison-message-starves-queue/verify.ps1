$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/NotificationQueueLab/NotificationQueueLab.csproj'
$output = (& dotnet run --project $project 2>&1) -join "`n"
Write-Host $output

$required = @(
  'Processed: 2',
  'DeadLettered: 1',
  'Pending: 0',
  'COMPLETE id=msg-good-1',
  'COMPLETE id=msg-good-2'
)

foreach ($text in $required) {
  if ($output -notlike "*$text*") {
    throw "Verification failed. Missing: $text"
  }
}

Write-Host 'VERIFIED: failed delivery reaches a terminal path and valid messages continue.'
