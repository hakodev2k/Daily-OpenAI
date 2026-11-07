$ErrorActionPreference='Stop'
$output = @(dotnet run --project (Join-Path $PSScriptRoot 'starter/Lab.csproj') 2>&1)
if ($LASTEXITCODE -ne 0) { throw "Starter failed: $($output -join '; ')" }
$lines = @($output | Where-Object { $_ -match '^recipient=' })
if ($lines.Count -ne 2 -or $lines[0] -ne 'recipient=shift-A;animal=owl' -or $lines[1] -ne 'recipient=shift-A;animal=fox') { throw "Symptom not reproduced: $($lines -join '; ')" }
Write-Host 'REPRODUCED: second shift notification has wrong recipient'
