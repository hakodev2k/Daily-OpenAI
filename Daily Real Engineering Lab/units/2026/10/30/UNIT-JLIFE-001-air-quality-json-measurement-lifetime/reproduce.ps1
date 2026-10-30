$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    $output = & dotnet run --project (Join-Path $PSScriptRoot 'starter/AirQualityLab.csproj') --configuration Release -- check 2>&1
    $exitCode = $LASTEXITCODE
    $text = $output | Out-String
    Write-Output $text
    if ($exitCode -eq 2 -and $text.Contains('LAB_RESULT=FAIL') -and $text.Contains('ObjectDisposedException') -and $text.Contains('PUBLISHED=0/2')) {
        Write-Output 'REPRODUCED=YES'
        exit 0
    }
    Write-Error 'Baseline symptom not reproduced; inspect SDK/build output and starter changes.'
    exit 1
}
finally { Pop-Location }
