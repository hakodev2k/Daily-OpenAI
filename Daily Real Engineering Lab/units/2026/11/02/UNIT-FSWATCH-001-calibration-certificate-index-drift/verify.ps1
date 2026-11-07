$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/CertificateIndexLab.csproj'
dotnet run --project $project --no-launch-profile
$exitCode = $LASTEXITCODE
if ($exitCode -ne 0) {
    Write-Error "Verification failed (exit $exitCode). Fix starter/CertificateIndexer.cs; solution code is not used."
    exit 1
}
Write-Host 'Verification passed: learner starter meets the full reconciliation contract.'
exit 0
