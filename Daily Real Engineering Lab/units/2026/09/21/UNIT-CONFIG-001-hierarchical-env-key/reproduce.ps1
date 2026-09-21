$ErrorActionPreference = "Stop"
$env:Downstream_TimeoutSeconds = "30"
Remove-Item Env:Downstream__TimeoutSeconds -ErrorAction SilentlyContinue
$out = dotnet run --project "$PSScriptRoot/starter/Starter.csproj"
$out
if ($out -notmatch "DEFAULT_STILL_ACTIVE") { throw "Expected the starter symptom was not reproduced." }
Write-Host "REPRODUCED"
