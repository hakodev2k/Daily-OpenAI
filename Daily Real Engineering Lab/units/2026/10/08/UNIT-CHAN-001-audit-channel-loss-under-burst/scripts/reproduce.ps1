$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
  dotnet run --project "./starter/AuditPipelineLab.csproj" -- reproduce
  if ($LASTEXITCODE -ne 0) { throw "Original symptom was not reproduced (or starter was modified)" }
} finally { Pop-Location }
