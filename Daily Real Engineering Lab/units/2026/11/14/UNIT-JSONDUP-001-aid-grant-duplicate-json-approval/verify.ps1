$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  dotnet build ./starter/GrantReplay.csproj --nologo
  if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
  dotnet run --no-build --project ./starter/GrantReplay.csproj -- --verify
  if ($LASTEXITCODE -ne 0) { throw 'Contract verification failed' }
  Write-Host 'PASS: learner-editable contract verified'
} finally { Pop-Location }
