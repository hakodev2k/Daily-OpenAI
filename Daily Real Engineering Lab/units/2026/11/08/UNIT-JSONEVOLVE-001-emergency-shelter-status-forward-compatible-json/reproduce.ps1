$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ShelterFeed.csproj'
$output = & dotnet run --project $project 2>&1 | Out-String
$code = $LASTEXITCODE
Write-Output $output
if ($code -ne 0 -and $output -notmatch 'IMPORTED=3') {
  Write-Output 'REPRODUCED: import failed before completing the batch'
  exit 0
}
Write-Error 'Original symptom not reproduced. This script is intended for the unmodified starter.'
exit 1
