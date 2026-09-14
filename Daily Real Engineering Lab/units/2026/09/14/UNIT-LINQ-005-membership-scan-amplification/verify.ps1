$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot 'starter/MembershipScanLab.csproj'
$output = & dotnet run --project $project -- --verify 2>&1
$output | ForEach-Object { Write-Host $_ }

if ($LASTEXITCODE -ne 0) {
    throw 'Verification failed. Preserve Matched=1000 while reducing comparison work below the required threshold.'
}

if (-not ($output | Where-Object { $_ -eq 'VERIFY_PASS' })) {
    throw 'Verification marker not found.'
}

Write-Host 'VERIFY_PASS: learner-editable starter preserves behavior and removes comparison amplification.'
