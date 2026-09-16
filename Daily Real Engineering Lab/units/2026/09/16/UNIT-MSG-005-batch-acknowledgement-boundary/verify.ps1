$ErrorActionPreference = 'Stop'
$output = dotnet run --project (Join-Path $PSScriptRoot 'starter/BatchAckLab.csproj') 2>&1
$output | ForEach-Object { Write-Host $_ }
$text = $output -join "`n"
if ($text -match 'FINAL order-101=1' -and $text -match 'FINAL order-102=1' -and $text -notmatch 'FINAL order-101=2' -and $text -notmatch 'FINAL order-102=2') {
    Write-Host 'VERIFIED: completed messages do not repeat their side effects after another delivery attempt.'
    exit 0
}
throw 'Verification failed: at least one previously successful message still repeated its side effect.'