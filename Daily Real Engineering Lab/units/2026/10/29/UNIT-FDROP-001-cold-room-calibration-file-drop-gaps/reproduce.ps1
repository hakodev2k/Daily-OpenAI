$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
    & dotnet build "starter/CertificateDropLab.csproj" -c Release --nologo
    if ($LASTEXITCODE -ne 0) { throw "Build failed" }
    $output = (& dotnet run --project "starter/CertificateDropLab.csproj" -c Release --no-build -- reproduce 2>&1 | Out-String)
    $exitCode = $LASTEXITCODE
    Write-Host $output
    if ($exitCode -eq 2 -and $output.Contains("REPRO_FAIL")) {
        Write-Host "REPRODUCED: shift-close register misses a valid file"
        exit 0
    }
    throw "Reproduction mismatch (exit=$exitCode). Restore the original starter if it was already fixed."
} finally { Pop-Location }
