$project = Join-Path $PSScriptRoot '../starter/JsonLab.csproj'
$output = & dotnet run --project $project --configuration Release 2>&1 | Out-String
$exitCode = $LASTEXITCODE
Write-Output $output
if ($exitCode -ne 0 -or $output -notmatch 'VERIFICATION_PASSED' -or $output -match 'CHECK_FAILED') {
    throw 'Verification failed: learner-editable starter did not satisfy all behavior checks.'
}
Write-Output 'LEARNER_VERIFICATION_PASSED'
