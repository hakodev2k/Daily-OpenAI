$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/DnsPoolLab.csproj -- --reproduce
    if ($LASTEXITCODE -ne 0) { throw "reproduce contract failed (exit code $LASTEXITCODE)." }
} finally { Pop-Location }
