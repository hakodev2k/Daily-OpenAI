$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  dotnet run --project ./starter/RescueLab.csproj -- --contract
  if ($LASTEXITCODE -ne 0) { exit 1 }
  exit 0
} finally { Pop-Location }
