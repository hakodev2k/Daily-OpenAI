$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/PooledBufferLab.csproj'

dotnet run --project $project
if ($LASTEXITCODE -ne 0) {
    throw 'Verification failed: caller-visible payload is still unstable or the project failed.'
}

Write-Host 'VERIFIED: learner-editable starter keeps the previously returned payload stable.'
