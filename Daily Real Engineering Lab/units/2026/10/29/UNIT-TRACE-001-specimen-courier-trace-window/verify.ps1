$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
& (Join-Path $root 'setup.ps1')
Push-Location $root
try {
    dotnet run --project 'starter/TraceWindowLab.csproj' --no-build -c Release -- verify
    if ($LASTEXITCODE -ne 0) { throw 'Learner starter failed behavior contract' }
} finally { Pop-Location }
