$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet --version
    if ($LASTEXITCODE -ne 0) { throw 'dotnet SDK 10 is required.' }
    dotnet restore ./src/LabHarness/LabHarness.csproj
    if ($LASTEXITCODE -ne 0) { throw 'Starter restore failed.' }
    dotnet restore ./solution/Reference.csproj
    if ($LASTEXITCODE -ne 0) { throw 'Reference restore failed.' }
    Write-Host 'Setup completed.'
}
finally { Pop-Location }
