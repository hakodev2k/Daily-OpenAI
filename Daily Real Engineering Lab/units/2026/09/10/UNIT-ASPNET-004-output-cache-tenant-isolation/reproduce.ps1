$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Starter.csproj'
$baseUrl = 'http://127.0.0.1:5187'

$process = Start-Process -FilePath 'dotnet' -ArgumentList @('run','--project',$project,'--no-launch-profile','--urls',$baseUrl) -PassThru

try {
    $ready = $false
    for ($i = 0; $i -lt 30; $i++) {
        try {
            Invoke-WebRequest -Uri "$baseUrl/health" -UseBasicParsing | Out-Null
            $ready = $true
            break
        }
        catch {
            Start-Sleep -Milliseconds 500
        }
    }

    if (-not $ready) {
        throw 'Starter did not become ready on port 5187.'
    }

    $first = Invoke-RestMethod -Uri "$baseUrl/flags" -Headers @{ 'X-Tenant-Id' = 'tenant-a' }
    $second = Invoke-RestMethod -Uri "$baseUrl/flags" -Headers @{ 'X-Tenant-Id' = 'tenant-b' }

    Write-Host "FIRST_TENANT=$($first.tenant)"
    Write-Host "SECOND_TENANT=$($second.tenant)"
    Write-Host "SECOND_PLAN=$($second.featurePlan)"

    if ($first.tenant -eq 'tenant-a' -and $second.tenant -eq 'tenant-a') {
        Write-Host 'REPRODUCED=true'
        exit 0
    }

    throw 'Expected the starter to reproduce cross-tenant response reuse, but it did not.'
}
finally {
    if ($process -and -not $process.HasExited) {
        Stop-Process -Id $process.Id -Force
    }
}
