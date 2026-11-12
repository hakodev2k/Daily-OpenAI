$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  $output = dotnet run --project starter/FerrySchedule.csproj
  if ($LASTEXITCODE -ne 0) { throw 'Starter execution failed' }
  $output | Write-Output
  $bad = @($output | Where-Object { $_ -match 'match=False' })
  $good = @($output | Where-Object { $_ -match 'match=True' })
  if ($bad.Count -ne 1 -or $good.Count -ne 1) { throw 'Expected one passing winter case and one failing summer case' }
  Write-Output 'REPRODUCED: seasonal UTC mismatch'
} finally { Pop-Location }
