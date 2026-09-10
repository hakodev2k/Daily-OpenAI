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
        throw 'Learner starter did not become ready on port 5187.'
    }

    $first = Invoke-RestMethod -Uri "$baseUrl/flags" -Headers @{ 'X-Tenant-Id' = 'tenant-a' }
    $second = Invoke-RestMethod -Uri "$baseUrl/flags" -Headers @{ 'X-Tenant-Id' = 'tenant-b' }

    if ($first.tenant -ne 'tenant-a' -or $first.featurePlan -ne 'Premium') {
        throw 'Tenant A behavior regressed.'
    }

    if ($second.tenant -ne 'tenant-b' -or $second.featurePlan -ne 'Standard') {
        throw "Tenant isolation failed. tenant=$($second.tenant), plan=$($second.featurePlan)"
    }

    Write-Host 'VERIFY=passed'
}
finally {
    if ($process -and -not $process.HasExited) {
        Stop-Process -Id $process.Id -Force
    }
}
