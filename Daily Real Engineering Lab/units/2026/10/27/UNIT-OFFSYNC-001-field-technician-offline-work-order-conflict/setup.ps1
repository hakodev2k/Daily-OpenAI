$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
  dotnet --version
  if ($LASTEXITCODE -ne 0) { throw "dotnet SDK not available" }
  dotnet restore ./starter/OfflineSyncLab.csproj
  if ($LASTEXITCODE -ne 0) { throw "restore failed" }
} finally { Pop-Location }
