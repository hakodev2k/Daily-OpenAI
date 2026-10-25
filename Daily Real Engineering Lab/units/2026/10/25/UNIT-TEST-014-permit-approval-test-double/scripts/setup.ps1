$ErrorActionPreference = "Stop"
Push-Location (Resolve-Path (Join-Path $PSScriptRoot ".."))
try {
    dotnet restore starter/PermitLab.csproj
    if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed" }
    dotnet build starter/PermitLab.csproj --no-restore
    if ($LASTEXITCODE -ne 0) { throw "dotnet build failed" }
}
finally { Pop-Location }
