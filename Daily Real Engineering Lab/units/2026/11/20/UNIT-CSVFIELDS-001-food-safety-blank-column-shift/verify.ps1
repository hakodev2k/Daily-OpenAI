$ErrorActionPreference = 'Stop'
Push-Location (Join-Path $PSScriptRoot 'starter')
try {
  dotnet run --project FoodLabImport.csproj --no-launch-profile -- --verify
  if ($LASTEXITCODE -ne 0) { throw 'Verification failed: check blank-field preservation and malformed record rejection.' }
  Write-Host 'Verification passed for learner starter code.'
} finally { Pop-Location }
