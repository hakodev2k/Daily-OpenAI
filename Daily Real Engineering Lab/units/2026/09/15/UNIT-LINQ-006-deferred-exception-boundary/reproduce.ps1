$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    dotnet run --project ./starter/DeferredBoundaryLab.csproj -- reproduce
    if ($LASTEXITCODE -ne 0) { throw "Starter symptom was not reproduced. Exit code: $LASTEXITCODE" }
    Write-Host "PASS: starter symptom reproduced."
}
finally { Pop-Location }
