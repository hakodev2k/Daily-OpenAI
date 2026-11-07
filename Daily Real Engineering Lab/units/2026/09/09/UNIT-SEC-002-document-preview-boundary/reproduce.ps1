$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot 'starter/DocumentPreview/DocumentPreview.csproj'

dotnet build $project --nologo
if ($LASTEXITCODE -ne 0) {
    throw 'Starter project did not build.'
}

$output = & dotnet run --project $project --no-build 2>&1
$exitCode = $LASTEXITCODE
$output | Write-Host

if ($exitCode -ne 2) {
    throw "Expected starter to reproduce the security symptom with exit code 2, actual: $exitCode"
}

if (-not ($output -match 'SECURITY_CHECK=FAILED')) {
    throw 'Expected SECURITY_CHECK=FAILED evidence was not produced.'
}

Write-Host 'REPRODUCTION_CONFIRMED: the original starter exposes data outside the intended preview boundary.'
