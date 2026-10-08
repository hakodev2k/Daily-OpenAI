$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
  dotnet --version
  if ($LASTEXITCODE -ne 0) { throw "dotnet SDK not found" }
  dotnet restore "./starter/AuditPipelineLab.csproj"
  if ($LASTEXITCODE -ne 0) { throw "restore failed" }
  dotnet build "./starter/AuditPipelineLab.csproj" --no-restore
  if ($LASTEXITCODE -ne 0) { throw "build failed" }
} finally { Pop-Location }
