$ErrorActionPreference = 'Stop'
Push-Location "$PSScriptRoot/starter"
try {
  dotnet run --project ClinicBooking.csproj
  if ($LASTEXITCODE -ne 0) { throw 'Learner starter still fails contract verification.' }
  Write-Host 'PASS: learner-modified starter satisfies all three contracts.'
} finally { Pop-Location }
