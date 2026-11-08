$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  $output = & dotnet run --project starter/GardenPlot.csproj 2>&1
  $exitCode = $LASTEXITCODE
  $output | ForEach-Object { Write-Output $_ }
  if ($exitCode -eq 1 -and (($output | Out-String) -match 'FAIL stale weak validator rejected')) { Write-Output 'REPRODUCED: stale update accepted'; exit 0 }
  Write-Error 'Expected original failure not reproduced (perhaps already fixed).'
  exit 1
} finally { Pop-Location }
