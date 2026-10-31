$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
dotnet run --project (Join-Path $root "solution/BikeWebhookSolution.csproj") -- verify
exit $LASTEXITCODE
