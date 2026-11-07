$ErrorActionPreference = 'Stop'
$unit = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Push-Location $unit
try {
    dotnet run --project ./harness/TenantQuotaHarness.csproj --no-restore -- --reproduce
    $exitCode = $LASTEXITCODE
} finally {
    Pop-Location
}
exit $exitCode
