$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../baseline/Baseline.csproj'
$output = & dotnet run --project $project -- --check 2>&1
$code = $LASTEXITCODE
$printed = $output | Out-String
Write-Output $printed
if ($code -ne 1 -or -not $printed.Contains('CONTRACT_FAIL')) {
    throw 'Baseline reproduction invalid: expected contract failures (not a build/runtime error).'
}
Write-Host 'REPRODUCED: baseline violates the vendor feed contract.'
