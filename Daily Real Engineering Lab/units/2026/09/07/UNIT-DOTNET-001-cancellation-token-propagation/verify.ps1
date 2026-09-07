$ErrorActionPreference = "Stop"

function Run-AndCapture([string]$project) {
    $output = dotnet run --project $project 2>&1 | Out-String
    if ($LASTEXITCODE -ne 0) { throw "dotnet run failed for $project" }
    return $output
}

$starter = Run-AndCapture (Join-Path $PSScriptRoot "starter/RealEngineeringLab.csproj")
$solution = Run-AndCapture (Join-Path $PSScriptRoot "solution/RealEngineeringLab.csproj")

if ($starter -notmatch "Downstream completed for CUS-001") { throw "Starter did not reproduce orphaned downstream work." }
if ($solution -notmatch "Request canceled after") { throw "Solution did not observe cancellation." }
if ($solution -match "Downstream completed for CUS-001") { throw "Solution still completed downstream work after cancellation." }

Write-Host "Verification passed."
