$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/NotificationQueueLab/NotificationQueueLab.csproj'
$output = (& dotnet run --project $project 2>&1) -join "`n"
Write-Host $output

$required = @(
  'Processed: 0',
  'DeadLettered: 0',
  'Pending: 3',
  'RECEIVE id=msg-poison delivery=6'
)

foreach ($text in $required) {
  if ($output -notlike "*$text*") {
    throw "Starter symptom was not reproduced. Missing: $text"
  }
}

Write-Host 'REPRODUCED: one failing message prevents queue progress in the observation window.'
