$ErrorActionPreference="Stop"
$o=dotnet run --project (Join-Path $PSScriptRoot "starter/Starter.csproj") 2>&1
$o | % { Write-Host $_ }
$t=$o -join "
"
if($t -notmatch "PAGE1=10,9,8" -or $t -notmatch "PAGE2=8,7,6"){ throw "Expected pagination drift was not reproduced." }
Write-Host "REPRODUCED"
