$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
    & dotnet build "starter/LiftPassLab.csproj" -c Release --nologo
    if ($LASTEXITCODE -ne 0) { throw "Build failed" }
    $output = (& dotnet run --project "starter/LiftPassLab.csproj" -c Release --no-build -- --verify 2>&1 | Out-String)
    $exitCode = $LASTEXITCODE
    Write-Host $output
    if ($exitCode -eq 0 -and $output.Contains("VERIFY_PASS") -and -not $output.Contains("VERIFY_FAIL")) {
        Write-Host "VERIFIED: learner-edited starter passes all token identity cases"
        exit 0
    }
    throw "Verification failed (exit=$exitCode); investigate starter/PartnerClient.cs"
} finally { Pop-Location }
