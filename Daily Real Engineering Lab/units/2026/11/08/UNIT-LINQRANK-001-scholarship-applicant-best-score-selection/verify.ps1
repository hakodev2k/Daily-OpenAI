$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  dotnet run --project ./starter/ScholarshipRank.csproj -- --verify
  if ($LASTEXITCODE -ne 0) { throw 'Verification failed: learner-editable starter still violates ranking contract.' }
  Write-Host 'VERIFIED: learner workspace passes ranking contract.'
} finally { Pop-Location }
