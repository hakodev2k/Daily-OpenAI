$ErrorActionPreference='Stop'
$p=Join-Path $PSScriptRoot '../starter/SensorFailover.csproj'
$a= & dotnet run --project $p --configuration Release -- 2>&1 | Out-String
$ac=$LASTEXITCODE
$b= & dotnet run --project $p --configuration Release -- all-failed 2>&1 | Out-String
$bc=$LASTEXITCODE
if($ac -ne 0 -or -not $a.Contains('DELIVERED:backup')){Write-Error "Failover failed: $a";exit 1}
if($bc -ne 0 -or -not $b.Contains('ALL_FAILED_HANDLED:2')){Write-Error "Outage handling failed: $b";exit 1}
Write-Output 'PASS';exit 0
