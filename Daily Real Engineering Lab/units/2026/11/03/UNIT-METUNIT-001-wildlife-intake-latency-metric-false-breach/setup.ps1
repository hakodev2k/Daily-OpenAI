$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    & dotnet --version
    if ($LASTEXITCODE -ne 0) { throw 'Missing .NET SDK.' }
    & dotnet restore 'starter/MetricLab.csproj'
    if ($LASTEXITCODE -ne 0) { throw 'Starter restore failed.' }
    & dotnet restore 'tests/MetricContract.csproj'
    if ($LASTEXITCODE -ne 0) { throw 'Contract restore failed.' }
} finally { Pop-Location }
