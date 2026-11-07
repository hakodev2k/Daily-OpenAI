$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./src/LabHarness/LabHarness.csproj -- reproduce
    if ($LASTEXITCODE -ne 0) { throw 'Expected starter symptom was not reproduced.' }
    Write-Host 'Reproduction confirmed (this is NOT a passing verification).'
}
finally { Pop-Location }
