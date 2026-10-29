$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    dotnet build './starter/MeterCodeLab.csproj' -c Release
    if ($LASTEXITCODE -ne 0) { throw 'build failed' }
    dotnet run --project './starter/MeterCodeLab.csproj' -c Release --no-build -- verify
    if ($LASTEXITCODE -ne 0) { throw 'Learner starter verification failed' }
} finally { Pop-Location }
