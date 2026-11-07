$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
    $lines = & dotnet run --project './starter/AttachmentScanLab.csproj' -c Release -- --check 2>&1
    $code = $LASTEXITCODE
    $lines | ForEach-Object { Write-Output $_ }
    $outputText = $lines -join [Environment]::NewLine
}
finally { Pop-Location }
if ($code -eq 0 -and $outputText.Contains('CAPACITY_8=PASS') -and $outputText.Contains('CAPACITY_1=PASS')) {
    Write-Output 'VERIFIED: learner starter passes both contracts'
    exit 0
}
Write-Error 'Verification failed.'
exit 1
