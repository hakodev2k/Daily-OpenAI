$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  $output = dotnet run --project starter/FerrySchedule.csproj
  if ($LASTEXITCODE -ne 0) { throw 'Learner code failed to run' }
  $output | Write-Output
  if (@($output | Where-Object { $_ -match 'match=False' }).Count -ne 0) { throw 'Verification failed: a seasonal case still differs' }
  if (@($output | Where-Object { $_ -match 'match=True' }).Count -ne 2) { throw 'Verification failed: expected two successful seasonal cases' }
  Write-Output 'PASS: both seasonal conversion cases'
} finally { Pop-Location }
