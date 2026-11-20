$ErrorActionPreference = 'Stop'
Push-Location (Join-Path $PSScriptRoot 'starter')
try {
  $output = & dotnet run --project TheaterTaxLab.csproj --no-launch-profile -- --verify
  if ($LASTEXITCODE -ne 0) { throw "Learner verification failed" }
  $output | ForEach-Object { Write-Output $_ }
  if (-not ($output -contains 'VERIFY_PASS')) { throw 'Missing VERIFY_PASS' }
  if (-not ($output -contains 'Delta=0.00')) { throw 'Expected reconciled Delta=0.00' }
} finally { Pop-Location }
