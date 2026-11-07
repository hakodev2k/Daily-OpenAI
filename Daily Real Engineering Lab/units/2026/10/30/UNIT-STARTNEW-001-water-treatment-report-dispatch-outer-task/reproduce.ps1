$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    $output = & dotnet run --project (Join-Path $PSScriptRoot 'harness/LabChecks.csproj') --configuration Release 2>&1
    $exitCode = $LASTEXITCODE
    $text = $output | Out-String
    Write-Output $text
    if ($exitCode -eq 2 -and $text.Contains('CASE=WAITS_FOR_SEND RESULT=FAIL') -and $text.Contains('CASE=PROPAGATES_FAILURE RESULT=FAIL') -and $text.Contains('LAB_RESULT=FAIL')) {
        Write-Output 'REPRODUCED=YES'
        exit 0
    }
    Write-Error 'Baseline was not reproduced; check SDK, build output, and whether starter was already edited.'
    exit 1
}
finally { Pop-Location }
