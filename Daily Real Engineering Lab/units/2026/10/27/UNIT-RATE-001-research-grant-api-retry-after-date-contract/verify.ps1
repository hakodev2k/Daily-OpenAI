$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/RetryWindowLab.csproj')
$code = $LASTEXITCODE
if ($code -ne 0) {
    throw "Verification failed: learner-editable starter returned $code; expected 8/8 PASS and exit 0."
}
Write-Host 'Verified: all eight starter fixtures passed.'
