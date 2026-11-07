$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/FreightContracts.csproj'
dotnet run --project $project -- --self-test
$code = $LASTEXITCODE
if ($code -eq 0) {
    Write-Error 'Starter unexpectedly passed all contract cases.'
    exit 1
}
Write-Host 'REPRODUCED: contract mismatches detected; inspect FAIL lines.'
exit 0
