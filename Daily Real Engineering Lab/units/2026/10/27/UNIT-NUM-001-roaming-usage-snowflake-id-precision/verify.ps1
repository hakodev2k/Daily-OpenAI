$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $false
$project = Join-Path $PSScriptRoot 'starter/RoamingUsage.csproj'
$output = & dotnet run --project $project -- --check 2>&1
$code = $LASTEXITCODE
$output | Out-Host
if ($code -ne 0 -or (($output | Out-String) -match 'CHECK_FAILED:') -or (([regex]::Matches(($output | Out-String), 'CHECK_OK:')).Count -ne 3)) {
    throw "Verification failed: expected all three behavior cases to pass on learner-edited starter (exit $code)."
}
Write-Host 'VERIFIED: learner starter passes all three behavior fixtures.'
