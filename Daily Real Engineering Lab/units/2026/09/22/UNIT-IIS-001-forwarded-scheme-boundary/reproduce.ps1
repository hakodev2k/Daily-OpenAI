$ErrorActionPreference = "Stop"
$env:CONNECTION_SCHEME = "http"
$env:FORWARDED_PROTO = "https"
$out = dotnet run --project "$PSScriptRoot/starter/Starter.csproj"
$out
if ($out -notmatch "REDIRECT_HTTPS") { throw "Expected redirect-loop symptom was not reproduced." }
Write-Host "REPRODUCED: externally HTTPS request is redirected again."
