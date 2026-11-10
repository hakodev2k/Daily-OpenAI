$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  dotnet run --project starter/RailGateway.csproj -- --verify
  if ($LASTEXITCODE -ne 0) { throw "Learner workspace verification failed (exit $LASTEXITCODE)." }
  Write-Host "All learner workspace contracts passed."
} finally { Pop-Location }
