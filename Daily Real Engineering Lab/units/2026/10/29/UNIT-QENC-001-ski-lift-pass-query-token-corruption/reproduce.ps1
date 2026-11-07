$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
    & dotnet build "starter/LiftPassLab.csproj" -c Release --nologo
    if ($LASTEXITCODE -ne 0) { throw "Build failed" }
    $output = (& dotnet run --project "starter/LiftPassLab.csproj" -c Release --no-build -- --reproduce 2>&1 | Out-String)
    $exitCode = $LASTEXITCODE
    Write-Host $output
    if ($exitCode -eq 2 -and $output.Contains("REPRO_FAIL")) {
        Write-Host "REPRODUCED: partner token differs from issued token"
        exit 0
    }
    throw "Reproduction mismatch (exit=$exitCode). Restore starter to original state if already fixed."
} finally { Pop-Location }
