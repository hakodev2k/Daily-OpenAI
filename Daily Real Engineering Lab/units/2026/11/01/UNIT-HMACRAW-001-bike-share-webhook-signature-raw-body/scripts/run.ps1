$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
dotnet run --project (Join-Path $root "starter/BikeWebhookLab.csproj") -- demo
exit $LASTEXITCODE
