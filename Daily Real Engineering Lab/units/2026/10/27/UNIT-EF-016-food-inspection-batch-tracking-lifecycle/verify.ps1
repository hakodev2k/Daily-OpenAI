$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  dotnet run --project ./starter/InspectionLab.csproj -- verify
  if ($LASTEXITCODE -ne 0) { throw 'Learner verification failed.' }
} finally { Pop-Location }
