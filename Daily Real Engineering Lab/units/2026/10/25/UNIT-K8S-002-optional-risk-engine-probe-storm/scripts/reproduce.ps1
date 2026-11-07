$ErrorActionPreference = "Stop"
Push-Location (Resolve-Path (Join-Path $PSScriptRoot ".."))
try {
    dotnet run --project starter/ProbeLab.csproj -- reproduce
    if ($LASTEXITCODE -ne 0) { throw "reproduce failed (exit $LASTEXITCODE)" }
}
finally { Pop-Location }
