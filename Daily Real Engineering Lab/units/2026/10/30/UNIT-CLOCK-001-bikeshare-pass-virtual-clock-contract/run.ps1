$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
    dotnet run --project "./harness/LabHarness.csproj" -- run
    $code = $LASTEXITCODE
} finally { Pop-Location }
exit $code
