$output = & dotnet run --project (Join-Path $PSScriptRoot 'starter/ManifestLab.csproj') 2>&1 | Out-String
$code = $LASTEXITCODE
Write-Host $output
if ($code -eq 0 -or $output -notmatch 'FAIL: historical manifest was mutated') { throw 'Reproduction contract failed' }
Write-Host 'REPRODUCED'
