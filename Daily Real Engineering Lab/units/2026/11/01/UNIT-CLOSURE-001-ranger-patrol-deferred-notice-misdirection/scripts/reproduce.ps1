$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
    dotnet run --project ./starter/PatrolLab.csproj -- reproduce
    if ($LASTEXITCODE -ne 0) { throw 'Baseline reproduction contract failed' }
}
finally { Pop-Location }
