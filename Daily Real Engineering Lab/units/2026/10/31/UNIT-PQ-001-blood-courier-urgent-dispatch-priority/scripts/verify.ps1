$ErrorActionPreference = 'Stop'
$unit = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$output = & dotnet run --project (Join-Path $unit 'starter/DispatchLab.csproj') --configuration Release 2>&1
$code = $LASTEXITCODE
$output | ForEach-Object { Write-Host $_ }
$transcript = $output -join "`n"
if ($code -eq 0 -and $transcript.Contains('LAB_RESULT:PASS')) {
    Write-Host 'VERIFIED: learner starter passes every contract.'
    exit 0
}
Write-Error "Verification failed (exit=$code). Fix starter/DispatchQueue.cs; do not copy the reference project into verification."
exit 1
