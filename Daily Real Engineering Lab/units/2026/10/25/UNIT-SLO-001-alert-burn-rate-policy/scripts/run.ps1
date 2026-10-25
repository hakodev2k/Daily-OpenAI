$ErrorActionPreference = "Stop"
Push-Location (Resolve-Path (Join-Path $PSScriptRoot ".."))
try {
    dotnet run --project starter/SloAlertLab.csproj -- observe
    if ($LASTEXITCODE -ne 0) { throw "observation failed ($LASTEXITCODE)" }
}
finally { Pop-Location }
