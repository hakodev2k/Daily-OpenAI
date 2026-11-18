$p=Join-Path $PSScriptRoot 'starter/Lab.csproj'
$a=& dotnet run --project $p -- --valid --assert 2>&1 | Out-String
if($LASTEXITCODE -ne 0 -or $a -notmatch 'AMOUNT=500'){throw "Valid failed: $a"}
$b=& dotnet run --project $p -- --assert 2>&1 | Out-String
if($LASTEXITCODE -eq 0 -or $b -match 'Ambiguous JSON accepted' -or $b -notmatch '(?i)duplicate'){throw "Ambiguous not rejected: $b"}
'PASS'