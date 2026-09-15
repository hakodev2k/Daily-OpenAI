$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    dotnet build ./starter/DeferredBoundaryLab.csproj --nologo
    if ($LASTEXITCODE -ne 0) { throw "Learner code does not build." }

    dotnet run --project ./starter/DeferredBoundaryLab.csproj --no-build -- verify
    if ($LASTEXITCODE -ne 0) { throw "Verification failed. Exit code: $LASTEXITCODE" }
    Write-Host "PASS: learner-editable starter preserves the required exception contract."
}
finally { Pop-Location }
