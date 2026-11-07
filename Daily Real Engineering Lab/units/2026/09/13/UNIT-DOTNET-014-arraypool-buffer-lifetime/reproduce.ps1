$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/PooledBufferLab.csproj'

dotnet run --project $project
$exitCode = $LASTEXITCODE

if ($exitCode -eq 1) {
    Write-Host 'REPRODUCED: payload previously returned to caller changed after buffer reuse.'
    exit 0
}

if ($exitCode -eq 0) {
    throw 'Expected starter failure was not reproduced. The starter may already have been fixed.'
}

throw "Starter failed unexpectedly with exit code $exitCode."
