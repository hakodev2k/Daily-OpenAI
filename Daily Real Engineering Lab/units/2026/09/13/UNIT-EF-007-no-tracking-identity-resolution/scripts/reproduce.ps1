$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot '..\starter\UnitEf007.csproj'

dotnet run --project $project
$code = $LASTEXITCODE

if ($code -eq 0) {
    throw 'Expected the starter state to reproduce the object-identity symptom, but it passed.'
}

if ($code -ne 1) {
    throw "Starter failed for an unexpected reason. Exit code: $code"
}

Write-Host 'Reproduction succeeded: functional values are correct, but CLR identity is duplicated.'
exit 0
