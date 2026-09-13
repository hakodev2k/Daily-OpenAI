$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ForwardedTrustLab.csproj'

Write-Host 'Reproducing original authorization failure...'
dotnet run --project $project

if ($LASTEXITCODE -eq 0) {
    throw 'Expected the starter to expose at least one incorrect authorization decision, but all scenarios passed.'
}

Write-Host 'Reproduction succeeded: starter contains an incorrect authorization decision.'
exit 0
