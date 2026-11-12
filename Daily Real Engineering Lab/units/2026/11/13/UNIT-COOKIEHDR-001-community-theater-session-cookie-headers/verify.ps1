$ErrorActionPreference = 'Stop'
$output = & dotnet run --project (Join-Path $PSScriptRoot 'starter/CookieLab.csproj') 2>&1
$code = $LASTEXITCODE
$output | ForEach-Object { Write-Output $_ }
if ($code -eq 0 -and (($output -join "`n") -match 'COOKIE_CONTRACT=PASS')) { Write-Output 'VERIFIED'; exit 0 }
Write-Error "Learner starter contract not satisfied (exit=$code)"; exit 1
