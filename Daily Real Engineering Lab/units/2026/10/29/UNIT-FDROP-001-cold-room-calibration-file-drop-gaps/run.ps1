$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
    & dotnet run --project "starter/CertificateDropLab.csproj" -c Release -- run
    if ($LASTEXITCODE -ne 0) { throw "Run failed" }
} finally { Pop-Location }
