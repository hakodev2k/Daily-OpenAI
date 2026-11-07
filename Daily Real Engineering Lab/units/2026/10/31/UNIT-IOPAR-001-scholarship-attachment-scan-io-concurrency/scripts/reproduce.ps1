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
if ($code -eq 1 -and $outputText.Contains('CAPACITY_8=FAIL')) {
    Write-Output 'REPRODUCED: capacity underutilized'
    exit 0
}
Write-Error 'Starter symptom not reproduced or build failed.'
exit 1
