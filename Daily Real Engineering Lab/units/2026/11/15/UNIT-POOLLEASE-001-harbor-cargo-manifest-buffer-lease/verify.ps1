$output = & dotnet run --project (Join-Path $PSScriptRoot 'starter/ManifestLab.csproj') 2>&1 | Out-String
$code = $LASTEXITCODE
Write-Host $output
if ($code -ne 0 -or $output -notmatch '(?m)^PASS\s*$' -or $output -match 'FAIL:') { throw 'Learner verification failed' }
Write-Host 'VERIFIED'
