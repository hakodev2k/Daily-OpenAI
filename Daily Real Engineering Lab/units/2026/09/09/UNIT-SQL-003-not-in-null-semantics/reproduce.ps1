$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/SqlNullSemanticsLab.csproj'

dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet build $project --no-restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$output = dotnet run --project $project --no-build 2>&1
$exitCode = $LASTEXITCODE
$output | ForEach-Object { Write-Host $_ }

if ($exitCode -eq 10 -and ($output -join "`n") -match 'BUSINESS_RULE_MATCH=False') {
    Write-Host 'REPRODUCED: starter query violates the expected business result.'
    exit 0
}

Write-Error 'Expected the original starter failure, but it was not reproduced. Reset starter/ before using reproduce.ps1.'
exit 1
