$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot 'starter/Lab.csproj'

dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet build $project --no-restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet run --project $project --no-build -- verify
if ($LASTEXITCODE -ne 0) {
    Write-Error 'Verification failed. The learner-editable starter path did not move to backend B after the refresh window.'
    exit 1
}

Write-Host 'Verification passed: client reuse is preserved and the rotated endpoint is observed after the refresh window.'
