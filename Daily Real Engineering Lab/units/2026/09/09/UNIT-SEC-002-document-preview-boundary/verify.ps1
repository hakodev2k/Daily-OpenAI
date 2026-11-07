$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot 'starter/DocumentPreview/DocumentPreview.csproj'

dotnet build $project --nologo
if ($LASTEXITCODE -ne 0) {
    throw 'Learner project did not build.'
}

$output = & dotnet run --project $project --no-build 2>&1
$exitCode = $LASTEXITCODE
$output | Write-Host

if ($exitCode -ne 0) {
    throw "Verification failed. Expected exit code 0 after the fix, actual: $exitCode"
}

if (-not ($output -match 'invoice: public-to-support')) {
    throw 'Regression: the valid file can no longer be previewed.'
}

if (-not ($output -match 'SECURITY_CHECK=PASSED')) {
    throw 'Boundary verification did not reject the outside file.'
}

if ($output -match 'salary: confidential') {
    throw 'Security regression: content from outside the allowed root was returned.'
}

Write-Host 'VERIFICATION_PASSED: valid preview works and the outside file is rejected.'
