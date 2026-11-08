$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  & dotnet run --project starter/GardenPlot.csproj
  if ($LASTEXITCODE -ne 0) { throw 'Verification failed: learner starter tests returned nonzero.' }
  Write-Output 'VERIFIED: learner implementation passes contracts.'
} finally { Pop-Location }
