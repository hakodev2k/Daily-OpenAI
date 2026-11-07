$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot 'starter/SecretRotationLab.csproj'

dotnet run --project $project
$exitCode = $LASTEXITCODE

if ($exitCode -ne 0) {
    throw 'Verification failed: learner-editable starter still sends a credential that the simulated provider rejects.'
}

Write-Host 'Verification passed: the learner-editable application observes the active credential and the request succeeds.'
exit 0
