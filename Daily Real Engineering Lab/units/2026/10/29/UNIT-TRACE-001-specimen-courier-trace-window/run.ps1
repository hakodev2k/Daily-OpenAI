$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
& (Join-Path $root 'setup.ps1')
Push-Location $root
try {
    dotnet run --project 'starter/TraceWindowLab.csproj' --no-build -c Release -- run
    if ($LASTEXITCODE -ne 0) { throw 'Runner failed' }
} finally { Pop-Location }
