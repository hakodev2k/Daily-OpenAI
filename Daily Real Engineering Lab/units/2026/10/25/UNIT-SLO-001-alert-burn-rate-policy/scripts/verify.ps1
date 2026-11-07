$ErrorActionPreference = "Stop"
Push-Location (Resolve-Path (Join-Path $PSScriptRoot ".."))
try {
    dotnet run --project starter/SloAlertLab.csproj -- verify
    if ($LASTEXITCODE -ne 0) { throw "learner policy did not satisfy contract ($LASTEXITCODE)" }
}
finally { Pop-Location }
