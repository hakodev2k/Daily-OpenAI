$ErrorActionPreference = "Stop"
$env:SIM_CONNECTION_SCHEME = "http"
$env:SIM_X_FORWARDED_PROTO = "https"
$out = dotnet run --project "$PSScriptRoot/starter/Starter.csproj"
$out
if ($out -notmatch "ACTION=REDIRECT_HTTPS") { throw "Expected proxy redirect symptom was not reproduced." }
Write-Host "REPRODUCED"