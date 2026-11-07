$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./src/LabHarness/LabHarness.csproj -- verify
    if ($LASTEXITCODE -ne 0) { throw 'Learner starter failed verification.' }
    Write-Host 'Learner starter passed verification.'
}
finally { Pop-Location }
