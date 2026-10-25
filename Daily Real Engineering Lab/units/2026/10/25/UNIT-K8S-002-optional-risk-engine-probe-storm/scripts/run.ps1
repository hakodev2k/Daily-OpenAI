$ErrorActionPreference = "Stop"
Push-Location (Resolve-Path (Join-Path $PSScriptRoot ".."))
try {
    dotnet run --project starter/ProbeLab.csproj -- demo
    if ($LASTEXITCODE -ne 0) { throw "run failed (exit $LASTEXITCODE)" }
}
finally { Pop-Location }
