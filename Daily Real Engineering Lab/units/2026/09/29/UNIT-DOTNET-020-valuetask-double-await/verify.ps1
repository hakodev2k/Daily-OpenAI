$root=Split-Path -Parent $MyInvocation.MyCommand.Path
$o=dotnet run --project "$root/starter/ValueTaskLab.csproj"|Out-String
Write-Host $o
if($o -notmatch "FAST: price=125"){throw "FAST incorrect"}
if($o -notmatch "SLOW: price=125"){throw "SLOW incorrect"}
if(($o|Select-String "telemetry.rate=1.25" -AllMatches).Matches.Count -ne 2){throw "Telemetry incorrect"}
if($o -match "InvalidOperationException"){throw "Async result still misused"}
Write-Host "VERIFY=PASS"
