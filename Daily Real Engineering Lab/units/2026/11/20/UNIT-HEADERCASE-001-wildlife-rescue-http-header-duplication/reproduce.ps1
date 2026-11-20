$ErrorActionPreference='Stop'
$out=@(dotnet run --project (Join-Path $PSScriptRoot 'starter/RescueGateway.csproj'))
if($LASTEXITCODE -ne 0){throw 'Build/run failed'}
$out | ForEach-Object {Write-Host $_}
if($out -notcontains 'conflict|ROUTE:north'){throw 'Intended failure not reproduced'}
Write-Host 'REPRODUCED'
