$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    $output = & dotnet run --project (Join-Path $PSScriptRoot 'starter/AirQualityLab.csproj') --configuration Release -- check 2>&1
    $exitCode = $LASTEXITCODE
    $text = $output | Out-String
    Write-Output $text
    if ($exitCode -eq 0 -and $text.Contains('LAB_RESULT=PASS') -and $text.Contains('PUBLISHED=2/2') -and $text.Contains('CASE=INVALID_JSON RESULT=PASS')) {
        Write-Output 'VERIFIED=YES'
        exit 0
    }
    Write-Error 'Verification failed: learner-editable starter must publish both readings and reject malformed JSON.'
    exit 1
}
finally { Pop-Location }
