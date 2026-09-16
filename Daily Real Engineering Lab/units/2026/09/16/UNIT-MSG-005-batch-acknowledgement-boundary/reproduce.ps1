$ErrorActionPreference = 'Stop'
$output = dotnet run --project (Join-Path $PSScriptRoot 'starter/BatchAckLab.csproj') 2>&1
$output | ForEach-Object { Write-Host $_ }
$text = $output -join "`n"
if ($text -match 'FINAL order-101=2' -and $text -match 'ATTEMPT 2: m1,m2,m3,m4') {
    Write-Host 'REPRODUCED: successful items from attempt 1 were delivered and applied again.'
    exit 0
}
throw 'Starter did not reproduce the expected duplicate-side-effect symptom.'