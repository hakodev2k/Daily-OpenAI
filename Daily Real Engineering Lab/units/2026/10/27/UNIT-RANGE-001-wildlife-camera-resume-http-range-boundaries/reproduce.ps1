$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/RangeLab/RangeLab.csproj -- --expect-broken
    if ($LASTEXITCODE -ne 0) { throw "Starter did not reproduce both expected failing fixtures (or failed to build)." }
    Write-Host "REPRODUCED: two multi-chunk fixtures have inconsistent bytes."
} finally { Pop-Location }
