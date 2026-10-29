$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    dotnet run --project './starter/BadgeDrainLab.csproj' -c Release -- verify
    if ($LASTEXITCODE -ne 0) { throw 'Learner-modified starter failed verification' }
} finally { Pop-Location }
