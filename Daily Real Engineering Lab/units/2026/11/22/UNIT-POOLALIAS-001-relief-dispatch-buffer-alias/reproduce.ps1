$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  dotnet run --project starter/DispatchLab.csproj
  if ($LASTEXITCODE -eq 1) { Write-Host 'REPRODUCED: starter violates payload integrity'; exit 0 }
  throw "Expected starter exit code 1, got $LASTEXITCODE. Do not use this script to verify a fix."
} finally { Pop-Location }
