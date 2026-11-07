$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot 'starter/Lab.csproj'

dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet run --project $project --no-restore -- reproduce
if ($LASTEXITCODE -ne 0) {
    Write-Error 'Expected starter symptom was not reproduced. Inspect the console output and local ports.'
    exit 1
}

Write-Host 'Reproduction confirmed: endpoint registry changed, but the next request still used the previous backend connection.'
