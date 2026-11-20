$ErrorActionPreference = 'Stop'
Push-Location (Join-Path $PSScriptRoot 'starter')
try {
  $output = & dotnet run --project TheaterTaxLab.csproj --no-launch-profile
  if ($LASTEXITCODE -ne 0) { throw "dotnet run failed" }
  $output | ForEach-Object { Write-Output $_ }
  $delta = @($output | Where-Object { $_ -eq 'Delta=-0.01' })
  if ($delta.Count -ne 1) { throw 'Starter symptom not reproduced: expected Delta=-0.01' }
  Write-Output 'REPRODUCED'
} finally { Pop-Location }
