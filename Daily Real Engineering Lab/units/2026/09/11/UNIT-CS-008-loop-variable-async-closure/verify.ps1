$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Lab.csproj'
$output = dotnet run --project $project
$code = $LASTEXITCODE
$output | ForEach-Object { Write-Host $_ }
if ($code -ne 0) {
  Write-Error "Verification failed: learner-editable starter returned exit code $code."
  exit 1
}
if (($output -join "`n") -notmatch 'alpha,bravo,charlie') {
  Write-Error 'Verification failed: expected each tenant exactly once.'
  exit 1
}
Write-Host 'PASS: all tenants were processed exactly once.'
exit 0
