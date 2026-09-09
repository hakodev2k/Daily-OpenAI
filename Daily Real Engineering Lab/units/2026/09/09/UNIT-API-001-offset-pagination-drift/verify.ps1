$ErrorActionPreference="Stop"
$o=dotnet run --project (Join-Path $PSScriptRoot "starter/Starter.csproj") 2>&1
$o | % { Write-Host $_ }
$t=$o -join "
"
if($t -notmatch "PAGE1=10,9,8"){ throw "Page1 changed unexpectedly." }
if($t -notmatch "PAGE2=7,6,5"){ throw "Page2 is not cursor-stable." }
Write-Host "PASS"
