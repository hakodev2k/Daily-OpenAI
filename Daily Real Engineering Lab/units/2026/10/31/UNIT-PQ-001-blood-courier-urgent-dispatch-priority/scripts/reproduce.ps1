$ErrorActionPreference = 'Stop'
$unit = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$output = & dotnet run --project (Join-Path $unit 'starter/DispatchLab.csproj') --configuration Release 2>&1
$code = $LASTEXITCODE
$output | ForEach-Object { Write-Host $_ }
$transcript = $output -join "`n"
if ($code -eq 1 -and $transcript.Contains('LAB_RESULT:FAIL')) {
    Write-Host 'REPRODUCED: starter violates at least one business contract.'
    exit 0
}
Write-Error "Reproduction not confirmed (exit=$code). Check build and harness output."
exit 1
