$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  dotnet run --project starter/DispatchLab.csproj
  if ($LASTEXITCODE -ne 0) { throw "Verification failed: learner workspace still violates payload integrity (exit $LASTEXITCODE)." }
  Write-Host 'VERIFIED: both dispatch payloads remain independent'
} finally { Pop-Location }
