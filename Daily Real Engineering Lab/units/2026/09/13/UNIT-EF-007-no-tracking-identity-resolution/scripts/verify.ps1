$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot '..\starter\UnitEf007.csproj'

dotnet run --project $project
$code = $LASTEXITCODE

if ($code -ne 0) {
    throw "Verification failed. Learner-editable starter still violates the required behavior. Exit code: $code"
}

Write-Host 'Verification passed: functional data is correct and one CLR instance represents the shared customer.'
