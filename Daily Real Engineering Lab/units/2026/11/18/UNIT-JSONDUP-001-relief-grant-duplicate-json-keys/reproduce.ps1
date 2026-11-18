$o=& dotnet run --project (Join-Path $PSScriptRoot 'starter/Lab.csproj') -- --assert 2>&1 | Out-String
if($LASTEXITCODE -eq 0 -or $o -notmatch 'Ambiguous JSON accepted'){throw "Unexpected: $o"}
'REPRODUCED'