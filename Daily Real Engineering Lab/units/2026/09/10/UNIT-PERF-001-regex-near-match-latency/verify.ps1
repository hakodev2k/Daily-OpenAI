$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/RegexIncident.csproj'
$output = dotnet run --project $project -- --verify 2>&1
$output | ForEach-Object { Write-Host $_ }
if (($LASTEXITCODE -ne 0) -or (-not ($output -match 'VERIFY_PASS'))) {
    throw 'Verification failed. The learner-editable starter path still violates correctness or performance expectations.'
}
