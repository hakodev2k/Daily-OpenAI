$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $root 'starter/SharedStateLab.csproj'

Write-Host '1/3 Restore + build starter'
dotnet restore $project
dotnet build $project --no-restore

Remove-Item Env:LAB_PARALLEL_GATE -ErrorAction SilentlyContinue
Write-Host '2/3 Run each test independently (both should pass)'
dotnet test $project --no-build --filter FullyQualifiedName~StripeRenewalTests
if ($LASTEXITCODE -ne 0) { throw 'Stripe test did not pass independently.' }
dotnet test $project --no-build --filter FullyQualifiedName~LegacyRenewalTests
if ($LASTEXITCODE -ne 0) { throw 'Legacy test did not pass independently.' }

Write-Host '3/3 Run both tests with controlled parallel gate (suite is expected to fail)'
$env:LAB_PARALLEL_GATE = '1'
dotnet test $project --no-build
$exitCode = $LASTEXITCODE
Remove-Item Env:LAB_PARALLEL_GATE -ErrorAction SilentlyContinue

if ($exitCode -eq 0) {
    throw 'Expected the original starter suite to expose cross-test interference, but it passed.'
}

Write-Host 'Reproduction succeeded: isolated tests passed, combined parallel suite failed as intended.'
exit 0
