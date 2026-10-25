$ErrorActionPreference = 'Stop'
$unit = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Push-Location $unit
try {
    dotnet restore ./harness/TenantQuotaHarness.csproj --nologo
    if ($LASTEXITCODE -ne 0) { throw 'dotnet restore failed' }
    dotnet build ./harness/TenantQuotaHarness.csproj --no-restore --nologo
    if ($LASTEXITCODE -ne 0) { throw 'dotnet build failed' }
    Write-Host 'SETUP_OK'
} finally {
    Pop-Location
}
