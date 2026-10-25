$ErrorActionPreference = "Stop"
Push-Location (Resolve-Path (Join-Path $PSScriptRoot ".."))
try {
    dotnet run --project starter/ProbeLab.csproj -- verify
    if ($LASTEXITCODE -ne 0) { throw "verify failed (exit $LASTEXITCODE)" }
}
finally { Pop-Location }
