$ErrorActionPreference = "Stop"

function Run-AndCapture([string]$project) {
    $output = dotnet run --project $project 2>&1 | Out-String
    if ($LASTEXITCODE -ne 0) { throw "dotnet run failed for $project" }
    return $output
}

$starter = Run-AndCapture (Join-Path $PSScriptRoot "starter/RealEngineeringLab.csproj")
$solution = Run-AndCapture (Join-Path $PSScriptRoot "solution/RealEngineeringLab.csproj")

if ($starter -notmatch "Inventory calls:\s*6") { throw "Starter reproduction failed." }
if ($solution -notmatch "Inventory calls:\s*3") { throw "Solution verification failed." }
if ($solution -notmatch "Eligible orders:\s*2") { throw "Functional behavior changed." }

Write-Host "Verification passed."
