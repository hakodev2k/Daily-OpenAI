$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
    dotnet run --project "./harness/LabHarness.csproj" -- verify
    $code = $LASTEXITCODE
} finally { Pop-Location }
exit $code
