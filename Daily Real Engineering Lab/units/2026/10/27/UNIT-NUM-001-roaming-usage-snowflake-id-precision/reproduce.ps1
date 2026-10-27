$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $false
$project = Join-Path $PSScriptRoot 'starter/RoamingUsage.csproj'
$output = & dotnet run --project $project -- --check 2>&1
$code = $LASTEXITCODE
$output | Out-Host
if ($code -ne 1 -or -not (($output | Out-String) -match 'CHECK_FAILED:')) {
    throw "Reproduction invalid: expected exit 1 with CHECK_FAILED marker, got $code. Build errors are not successful reproductions."
}
Write-Host 'EXPECTED FAILURE reproduced. Investigate starter/UsageImporter.cs before opening solution.'
