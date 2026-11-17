$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  $output = & dotnet run --project ./starter/WildlifeSurvey.csproj 2>&1 | Out-String
  if ($output -notmatch 'INGEST_ACCEPTED' -or $output -notmatch 'ObjectDisposedException') { throw "Expected failure was not reproduced. Output: $output" }
  Write-Host 'REPRODUCED: worker failed after successful ingest.'
} finally { Pop-Location }
