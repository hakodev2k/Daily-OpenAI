$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ShelterQuotaLab.csproj'
$PSNativeCommandUseErrorActionPreference = $false
$output = & dotnet run --project $project --configuration Release 2>&1
$exitCode = $LASTEXITCODE
$output | Out-Host
if ($exitCode -eq 0 -and (($output -join [Environment]::NewLine) -match 'VERIFIED: all six')) {
    Write-Host 'VERIFIED: learner-edited starter passes the live-policy contract.'
    exit 0
}
Write-Error "Verification failed. ExitCode=$exitCode"
exit 1
