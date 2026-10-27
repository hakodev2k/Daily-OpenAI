$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
  dotnet run --project ./starter/OfflineSyncLab.csproj --no-launch-profile -- --reproduce
  if ($LASTEXITCODE -ne 0) { throw "Expected starter failure signature was not reproduced. Restore starter/SyncEngine.cs before reproducing." }
} finally { Pop-Location }
