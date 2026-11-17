$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  $output = & dotnet run --project ./starter/WildlifeSurvey.csproj 2>&1 | Out-String
  if ($LASTEXITCODE -ne 0 -or $output -notmatch 'INGEST_ACCEPTED' -or $output -notmatch 'WORKER_OK CAM-42 pangolin') { throw "Verification failed: $output" }
  Write-Host 'PASS: learner starter path processes the survey event.'
} finally { Pop-Location }
