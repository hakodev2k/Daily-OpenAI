$ErrorActionPreference = "Stop"
Push-Location (Resolve-Path (Join-Path $PSScriptRoot ".."))
try {
    dotnet restore starter/ProbeLab.csproj
    if ($LASTEXITCODE -ne 0) { throw "restore failed" }
    dotnet build starter/ProbeLab.csproj --no-restore
    if ($LASTEXITCODE -ne 0) { throw "build failed" }
}
finally { Pop-Location }
