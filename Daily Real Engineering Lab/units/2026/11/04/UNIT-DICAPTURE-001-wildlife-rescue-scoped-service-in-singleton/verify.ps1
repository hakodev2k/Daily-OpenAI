$ErrorActionPreference='Stop'
$output = @(dotnet run --project (Join-Path $PSScriptRoot 'starter/Lab.csproj') 2>&1)
if ($LASTEXITCODE -ne 0) { throw "Learner starter failed: $($output -join '; ')" }
$lines = @($output | Where-Object { $_ -match '^recipient=' })
if ($lines.Count -ne 2 -or $lines[0] -ne 'recipient=shift-A;animal=owl' -or $lines[1] -ne 'recipient=shift-B;animal=fox') { throw "Verification failed: $($lines -join '; ')" }
Write-Host 'PASS: each shift receives its own notification'
