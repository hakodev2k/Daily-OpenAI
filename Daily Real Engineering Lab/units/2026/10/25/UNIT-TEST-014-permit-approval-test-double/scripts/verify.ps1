$ErrorActionPreference = "Stop"
Push-Location (Resolve-Path (Join-Path $PSScriptRoot ".."))
try {
    dotnet run --project starter/PermitLab.csproj -- verify
    if ($LASTEXITCODE -ne 0) { throw "learner code failed verification (exit $LASTEXITCODE)" }
}
finally { Pop-Location }
