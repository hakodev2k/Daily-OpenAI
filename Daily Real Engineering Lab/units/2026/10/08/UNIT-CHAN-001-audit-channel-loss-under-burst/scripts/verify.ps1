$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
  # Must validate learner-edited starter, never solution code.
  dotnet run --project "./starter/AuditPipelineLab.csproj" -- verify
  if ($LASTEXITCODE -ne 0) { throw "Learner starter verification failed" }
} finally { Pop-Location }
