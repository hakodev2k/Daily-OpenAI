$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/DnsPoolLab.csproj -- --verify
    if ($LASTEXITCODE -ne 0) { throw "verify contract failed (exit code $LASTEXITCODE)." }
} finally { Pop-Location }
