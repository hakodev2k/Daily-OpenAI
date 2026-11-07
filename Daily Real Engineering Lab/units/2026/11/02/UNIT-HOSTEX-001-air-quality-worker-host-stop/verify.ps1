$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/AirAlertLab.csproj'
dotnet run --project $project --no-launch-profile
$exitCode = $LASTEXITCODE
if ($exitCode -ne 0) {
    Write-Error "Verification failed (exit $exitCode). Edit starter/NoticeWorker.cs; do not substitute solution/ code in the test path."
    exit 1
}
Write-Host 'Verification passed: learner-edited starter satisfies the delivery and host-lifetime contract.'
exit 0
