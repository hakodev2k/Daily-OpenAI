$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot '..\starter\DeployPathLab.csproj'

dotnet run --project $project
$exit = $LASTEXITCODE

if ($exit -eq 0) {
    throw 'Expected the starter state to reproduce the lookup failure, but it succeeded.'
}

Write-Host 'Reproduction confirmed: deployed entry exists but configured lookup fails.'
exit 0
