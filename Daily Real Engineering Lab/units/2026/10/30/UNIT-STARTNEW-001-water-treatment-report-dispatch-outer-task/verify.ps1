$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    $output = & dotnet run --project (Join-Path $PSScriptRoot 'harness/LabChecks.csproj') --configuration Release 2>&1
    $exitCode = $LASTEXITCODE
    $text = $output | Out-String
    Write-Output $text
    if ($exitCode -eq 0 -and $text.Contains('CASE=WAITS_FOR_SEND RESULT=PASS') -and $text.Contains('CASE=PROPAGATES_FAILURE RESULT=PASS') -and $text.Contains('CASE=SUCCESS_PATH RESULT=PASS') -and $text.Contains('LAB_RESULT=PASS')) {
        Write-Output 'VERIFIED=YES'
        exit 0
    }
    Write-Error 'Verification failed: modify starter/ReportDispatcher.cs, not harness/.'
    exit 1
}
finally { Pop-Location }
