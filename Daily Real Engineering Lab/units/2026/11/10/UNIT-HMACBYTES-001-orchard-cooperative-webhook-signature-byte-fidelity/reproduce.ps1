$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  dotnet run --project ./starter/WebhookLab.csproj --configuration Release -- --mode reproduce
  if ($LASTEXITCODE -ne 0) { throw "Lab reproduce failed (exit $LASTEXITCODE)" }
} finally { Pop-Location }
