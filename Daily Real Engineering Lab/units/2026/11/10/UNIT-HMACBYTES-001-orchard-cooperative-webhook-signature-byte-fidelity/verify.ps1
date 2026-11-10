$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  dotnet run --project ./starter/WebhookLab.csproj --configuration Release -- --mode verify
  if ($LASTEXITCODE -ne 0) { throw "Lab verify failed (exit $LASTEXITCODE)" }
} finally { Pop-Location }
