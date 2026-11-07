$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ShelterQuotaLab.csproj'
$PSNativeCommandUseErrorActionPreference = $false
$output = & dotnet run --project $project --configuration Release 2>&1
$exitCode = $LASTEXITCODE
$output | Out-Host
if ($exitCode -eq 23 -and (($output -join [Environment]::NewLine) -match 'CONFIG_DRIFT_DETECTED')) {
    Write-Host 'REPRODUCED: starter exhibits expected live-configuration mismatch.'
    exit 0
}
Write-Error "Reproduction did not match original contract. ExitCode=$exitCode"
exit 1
