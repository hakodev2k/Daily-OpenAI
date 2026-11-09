$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  $output = @(dotnet run --project ./starter/RescueLab.csproj --)
  if ($LASTEXITCODE -ne 0) { exit 2 }
  $output | ForEach-Object { Write-Output $_ }
  if (($output -match 'revised CRITICAL severity=1').Count -gt 0 -and ($output -match 'dispatched DispatchTicket \{ IncidentId = ROUTINE').Count -gt 0) { Write-Output 'REPRODUCED'; exit 0 }
  Write-Error 'Expected symptom not observed'; exit 1
} finally { Pop-Location }
