$ErrorActionPreference = 'Stop'
$unit = Split-Path $PSScriptRoot -Parent
$output = & dotnet run --project (Join-Path $unit 'starter/HarborManifest.csproj') -- --reproduce 2>&1
$code = $LASTEXITCODE
$output | ForEach-Object { Write-Output $_ }
if ($code -eq 1 -and (($output -join ' ') -match 'equivalent=True; hashEqual=False')) { Write-Output 'REPRODUCED: comparer contract mismatch'; exit 0 }
Write-Error "Reproduction failed or starter already fixed (exit=$code)"
exit 1
