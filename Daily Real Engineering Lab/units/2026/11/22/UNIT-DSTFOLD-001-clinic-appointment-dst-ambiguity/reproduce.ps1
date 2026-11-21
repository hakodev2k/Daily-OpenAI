$ErrorActionPreference = 'Stop'
Push-Location "$PSScriptRoot/starter"
try {
  dotnet run --project ClinicBooking.csproj
  if ($LASTEXITCODE -eq 0) { throw 'Starter unexpectedly passed all contracts.' }
  Write-Host 'PASS: original starter reproduces a contract failure.'
} finally { Pop-Location }
