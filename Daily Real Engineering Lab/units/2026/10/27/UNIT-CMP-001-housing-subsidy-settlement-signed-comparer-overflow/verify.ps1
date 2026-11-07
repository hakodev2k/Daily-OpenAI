$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/SettlementOrderLab.csproj'
& dotnet run --project $project -- --scenario all
$code = $LASTEXITCODE
if ($code -ne 0) {
    throw "VERIFY FAILED (exit $code): cần PASS cả ordinary, boundary và tie trên starter đã sửa."
}
Write-Host 'VERIFIED: all ordering-contract scenarios passed on learner starter.'
