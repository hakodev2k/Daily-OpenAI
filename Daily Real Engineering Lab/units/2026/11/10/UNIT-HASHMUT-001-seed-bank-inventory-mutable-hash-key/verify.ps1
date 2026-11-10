$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/SeedBank.csproj"
dotnet run --project $project -- verify
if ($LASTEXITCODE -ne 0) { throw "Verification failed: learner starter does not satisfy collection invariants." }
Write-Host "PASS: learner-editable starter verified"
