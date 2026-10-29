$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    dotnet build './starter/MonthlyScheduleLab.csproj' -c Release
    if ($LASTEXITCODE -ne 0) { throw 'dotnet build failed' }
    dotnet run --project './starter/MonthlyScheduleLab.csproj' -c Release --no-build -- run
    if ($LASTEXITCODE -ne 0) { throw 'run contract failed' }
} finally { Pop-Location }
