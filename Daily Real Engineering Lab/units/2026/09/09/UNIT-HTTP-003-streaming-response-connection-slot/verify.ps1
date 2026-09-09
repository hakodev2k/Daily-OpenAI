$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Lab.csproj'

Write-Host 'Building learner workspace...'
dotnet build $project --nologo
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host 'Verifying learner fix...'
$output = & dotnet run --project $project --no-build --nologo 2>&1 | Out-String
$exitCode = $LASTEXITCODE
Write-Host $output

if ($exitCode -ne 0) {
    Write-Error "Learner code exited with $exitCode."
    exit 1
}

if ($output -match 'SYMPTOM:') {
    Write-Error 'The original symptom is still present.'
    exit 1
}

if ($output -notmatch 'Request 3 completed: 200') {
    Write-Error 'Request 3 did not complete successfully within the verification path.'
    exit 1
}

Write-Host 'Verification passed: request 3 completes without increasing the configured connection limit.'
exit 0
