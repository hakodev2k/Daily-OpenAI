$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"
$output = dotnet run --project $project 2>&1
$output | ForEach-Object { Write-Host $_ }

if ($LASTEXITCODE -ne 0) {
    Write-Error "REPRODUCE FAIL: starter did not run successfully."
    exit 1
}

$text = $output -join "`n"
if ($text -notmatch "EFFECTIVE=https://payments.staging.example/") {
    Write-Error "REPRODUCE FAIL: expected stale staging override to win."
    exit 1
}
if ($text -notmatch "EnvironmentVariablesConfigurationProvider") {
    Write-Error "REPRODUCE FAIL: expected debug view to expose environment provider provenance."
    exit 1
}

Write-Host "REPRODUCED: deployment environment overrides versioned production configuration."
