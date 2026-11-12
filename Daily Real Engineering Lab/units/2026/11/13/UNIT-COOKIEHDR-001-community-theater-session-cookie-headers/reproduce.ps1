$ErrorActionPreference = 'Stop'
$output = & dotnet run --project (Join-Path $PSScriptRoot 'starter/CookieLab.csproj') 2>&1
$code = $LASTEXITCODE
$output | ForEach-Object { Write-Output $_ }
if ($code -eq 2 -and (($output -join "`n") -match 'COOKIE_CONTRACT=FAIL')) { Write-Output 'REPRODUCED'; exit 0 }
Write-Error "Expected original failure, got exit=$code"; exit 1
