$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
    dotnet --version
    if ($LASTEXITCODE -ne 0) { throw "dotnet SDK not available" }
    dotnet restore "./harness/LabHarness.csproj"
    if ($LASTEXITCODE -ne 0) { throw "restore failed" }
    dotnet build "./harness/LabHarness.csproj" --no-restore
    if ($LASTEXITCODE -ne 0) { throw "build failed" }
} finally { Pop-Location }
