$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/CultureIdentifierLab.csproj'

dotnet run --project $project -- reproduce
$exit = $LASTEXITCODE

if ($exit -ne 2) {
    throw "Expected the original starter to reproduce the lookup failure with exit code 2, got $exit."
}

Write-Host 'Reproduction confirmed: the technical identifier did not match under the simulated culture.'
exit 0
