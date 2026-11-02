$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/AirAlertLab.csproj'
dotnet run --project $project --no-launch-profile
$exitCode = $LASTEXITCODE
if ($exitCode -ne 3) {
    Write-Error "Reproduction failed: expected starter exit code 3 (LAB_RESULT=EXPECTED_FAILURE), got $exitCode. If you already fixed the starter, use verify.ps1."
    exit 1
}
Write-Host 'Reproduction confirmed: original starter fails as designed.'
exit 0
