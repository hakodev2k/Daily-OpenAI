$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./src/LabHarness/LabHarness.csproj -- demo
    if ($LASTEXITCODE -ne 0) { throw 'Demo failed.' }
}
finally { Pop-Location }
