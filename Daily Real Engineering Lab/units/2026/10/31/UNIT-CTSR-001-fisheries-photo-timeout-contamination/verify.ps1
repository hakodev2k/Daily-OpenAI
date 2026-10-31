$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet run --project (Join-Path $root 'starter/CoastalInspection.csproj') -- verify
if ($LASTEXITCODE -ne 0) { Write-Error 'Learner-editable starter failed regression contract'; exit 2 }
Write-Host 'REGRESSION CONTRACT PASS'
