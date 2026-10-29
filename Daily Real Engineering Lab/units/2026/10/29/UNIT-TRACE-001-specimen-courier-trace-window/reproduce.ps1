$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
& (Join-Path $root 'setup.ps1')
Push-Location $root
try {
    dotnet run --project 'starter/TraceWindowLab.csproj' --no-build -c Release -- reproduce
    if ($LASTEXITCODE -ne 0) { throw 'Baseline symptom not reproduced' }
} finally { Pop-Location }
