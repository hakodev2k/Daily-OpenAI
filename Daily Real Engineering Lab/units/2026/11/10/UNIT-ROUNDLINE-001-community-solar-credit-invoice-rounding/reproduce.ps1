$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
  $output = & dotnet run --project starter/SolarCredits.csproj 2>&1
  if ($LASTEXITCODE -ne 0) { throw "Starter execution failed: $output" }
  $output | ForEach-Object { Write-Output $_ }
  $line = $output | Where-Object { $_ -match '^MISMATCHES=([0-9]+)$' } | Select-Object -Last 1
  if (-not $line) { throw "Missing diagnostic marker" }
  $count = [int]($line -replace '^MISMATCHES=', '')
  if ($count -lt 1) { throw "Expected starter symptom not reproduced" }
  Write-Output "REPRODUCED: $count contract mismatches"
} finally { Pop-Location }
