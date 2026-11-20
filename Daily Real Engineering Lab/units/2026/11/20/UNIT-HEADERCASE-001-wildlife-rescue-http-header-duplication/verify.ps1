$ErrorActionPreference='Stop'
$out=@(dotnet run --project (Join-Path $PSScriptRoot 'starter/RescueGateway.csproj'))
if($LASTEXITCODE -ne 0){throw 'Build/run failed'}
$expected=@('single|ROUTE:north','same-duplicate|REJECT','conflict|REJECT','missing|REJECT','empty|REJECT')
foreach($line in $expected){if($out -notcontains $line){throw "Expected $line; actual: $($out -join '; ')"}}
if($out.Count -ne $expected.Count){throw 'Unexpected output count'}
Write-Host 'PASS: learner starter validated'
