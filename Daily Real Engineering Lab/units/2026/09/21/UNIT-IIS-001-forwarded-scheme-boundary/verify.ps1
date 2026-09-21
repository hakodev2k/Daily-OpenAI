$ErrorActionPreference = "Stop"
$env:SIM_CONNECTION_SCHEME = "http"
$env:SIM_X_FORWARDED_PROTO = "https"
$proxied = dotnet run --project "$PSScriptRoot/starter/Starter.csproj"
$proxied
if ($proxied -notmatch "Effective scheme: https" -or $proxied -notmatch "ACTION=SERVE_REQUEST") { throw "FAIL: trusted proxied HTTPS request is still treated as HTTP." }
$env:SIM_CONNECTION_SCHEME = "http"
$env:SIM_X_FORWARDED_PROTO = ""
$direct = dotnet run --project "$PSScriptRoot/starter/Starter.csproj"
$direct
if ($direct -notmatch "ACTION=REDIRECT_HTTPS") { throw "FAIL: true HTTP request no longer requires HTTPS." }
Write-Host "PASS"