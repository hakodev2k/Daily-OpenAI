$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/DnsPoolLab.csproj -- --run
    if ($LASTEXITCODE -ne 0) { throw "run contract failed (exit code $LASTEXITCODE)." }
} finally { Pop-Location }
