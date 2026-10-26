$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/RetryWindowLab.csproj')
$code = $LASTEXITCODE
if ($code -ne 12) {
    throw "Reproduction failed: expected harness mismatch exit 12, got $code. Build/setup errors are not valid reproduction."
}
Write-Host 'Reproduced: original starter violates fixture contract (exit 12).'
