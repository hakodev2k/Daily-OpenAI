$ErrorActionPreference = "Stop"
Remove-Item Env:Downstream_TimeoutSeconds -ErrorAction SilentlyContinue
$env:Downstream__TimeoutSeconds = "30"
$out = dotnet run --project "$PSScriptRoot/starter/Starter.csproj"
$out
if ($out -notmatch "OVERRIDE_APPLIED") { throw "FAIL: learner implementation does not honor the hierarchical deployment override." }
if ($out -notmatch "Downstream timeout: 30s") { throw "FAIL: expected timeout 30s." }
Write-Host "PASS"
