$ErrorActionPreference = 'Stop'
$unit = Split-Path $PSScriptRoot -Parent
$output = & dotnet run --project (Join-Path $unit 'starter/HarborManifest.csproj') -- --verify 2>&1
$code = $LASTEXITCODE
$output | ForEach-Object { Write-Output $_ }
if ($code -eq 0 -and (($output -join ' ') -match 'unique=2; expected=2') -and (($output -join ' ') -match 'lookup=True')) { Write-Output 'PASS: learner starter verified'; exit 0 }
Write-Error "FAIL: learner starter still violates contract (exit=$code)"
exit 1
