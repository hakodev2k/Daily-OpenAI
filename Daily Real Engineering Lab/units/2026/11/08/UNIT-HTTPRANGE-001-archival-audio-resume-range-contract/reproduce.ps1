$ErrorActionPreference = 'Stop'
Push-Location "$PSScriptRoot/starter"
try {
  dotnet build --nologo | Out-Host
  dotnet run --no-build -- honored | Out-Host
  if ($LASTEXITCODE -ne 0) { throw 'Baseline honored-range must succeed.' }
  dotnet run --no-build -- ignored | Out-Host
  if ($LASTEXITCODE -ne 2) { throw 'Expected corrupt resume when server ignores Range (exit 2).' }
  Write-Host 'REPRODUCED: ignored Range corrupts resumed bytes.'
} finally { Pop-Location }
