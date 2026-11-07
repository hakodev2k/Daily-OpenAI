$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Starter.csproj'

dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet build $project --no-restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$output = & dotnet run --project $project --no-build 2>&1 | Out-String
$exitCode = $LASTEXITCODE
Write-Host $output

if ($exitCode -ne 42) {
    Write-Error "Expected starter to exit with code 42, actual: $exitCode"
    exit 1
}

if ($output -notmatch 'OBJECT_DISPOSED') {
    Write-Error 'Expected OBJECT_DISPOSED marker was not found.'
    exit 1
}

Write-Host 'Reproduction confirmed: query fails only when consumed after repository returns.'
exit 0
