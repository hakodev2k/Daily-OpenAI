$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
    & dotnet build "starter/RequestBodyReplayLab.csproj" -c Release --nologo
    if ($LASTEXITCODE -ne 0) { throw "Build failed; not a valid reproduction" }
    $output = (& dotnet run --project "starter/RequestBodyReplayLab.csproj" -c Release --no-build -- --reproduce 2>&1 | Out-String)
    $exitCode = $LASTEXITCODE
    Write-Host $output
    if ($exitCode -eq 2 -and $output.Contains("REPRO_FAIL") -and $output.Contains("endpointBytes=0")) {
        Write-Host "REPRODUCED: valid audit payload is absent at endpoint"
        exit 0
    }
    throw "Reproduction mismatch (exit=$exitCode); inspect fixture and starter"
} finally { Pop-Location }
