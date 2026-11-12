$root = Split-Path $PSScriptRoot -Parent
$output = & dotnet run --project (Join-Path $root 'starter/ParcelLab.csproj') 2>&1
$exitCode = $LASTEXITCODE
$output | ForEach-Object { Write-Output $_ }
if ($exitCode -eq 1 -and (($output -join "`n") -match 'FAIL: parcel not retrievable')) { Write-Output 'REPRODUCED: expected starter failure'; exit 0 }
Write-Error "Reproduction mismatch (exit=$exitCode). Reset starter or inspect output."
exit 1
