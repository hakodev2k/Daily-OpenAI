$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
  dotnet run --project ./starter/OfflineSyncLab.csproj --no-launch-profile
  if ($LASTEXITCODE -ne 0) { throw "run failed" }
} finally { Pop-Location }
