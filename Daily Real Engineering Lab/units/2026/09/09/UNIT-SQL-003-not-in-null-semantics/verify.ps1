$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/SqlNullSemanticsLab.csproj'

dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet build $project --no-restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$output = dotnet run --project $project --no-build 2>&1
$exitCode = $LASTEXITCODE
$output | ForEach-Object { Write-Host $_ }
$text = $output -join "`n"

$idsOk = $text.Contains('OBSERVED_ELIGIBLE_IDS=1,3')
$ruleOk = $text.Contains('BUSINESS_RULE_MATCH=True')

if ($exitCode -eq 0 -and $idsOk -and $ruleOk) {
    Write-Host 'VERIFIED: starter satisfies the expected business result.'
    exit 0
}

Write-Error 'Verification failed. Expected eligible IDs 1,3.'
exit 1
