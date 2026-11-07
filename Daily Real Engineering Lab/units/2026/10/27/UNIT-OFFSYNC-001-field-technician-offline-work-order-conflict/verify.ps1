$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
  dotnet run --project ./starter/OfflineSyncLab.csproj --no-launch-profile -- --verify
  if ($LASTEXITCODE -ne 0) { throw "Learner code did not pass offline sync contract. Investigate starter/SyncEngine.cs." }
} finally { Pop-Location }
