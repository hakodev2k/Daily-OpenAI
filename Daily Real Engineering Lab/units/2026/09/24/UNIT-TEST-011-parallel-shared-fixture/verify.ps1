$ErrorActionPreference = 'Stop'
for ($i = 0; $i -lt 8; $i++) {
  dotnet run --project "$PSScriptRoot/starter/ParallelFixtureLab.csproj"
  if ($LASTEXITCODE -ne 0) { throw "Verification failed on iteration $i." }
}
Write-Host 'Verification passed: learner-editable starter remained stable across repeated parallel runs.'