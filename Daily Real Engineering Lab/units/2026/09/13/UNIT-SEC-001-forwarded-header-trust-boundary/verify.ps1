$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ForwardedTrustLab.csproj'

Write-Host 'Verifying learner-modified starter...'
dotnet run --project $project

if ($LASTEXITCODE -ne 0) {
    throw 'Verification failed. At least one security or legitimate-proxy scenario still has the wrong authorization decision.'
}

Write-Host 'Verification passed: attack case is rejected and legitimate trusted-proxy cases behave as expected.'
