$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
dotnet restore (Join-Path $root "starter/BikeWebhookLab.csproj")
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet restore (Join-Path $root "solution/BikeWebhookSolution.csproj")
exit $LASTEXITCODE
