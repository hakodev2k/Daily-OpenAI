$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/SettlementOrderLab.csproj'
& dotnet run --project $project -- --scenario boundary
$code = $LASTEXITCODE
if ($code -ne 2) {
    throw "Expected starter failure exit code 2, received $code. Hãy chạy trước khi sửa starter."
}
Write-Host 'REPRODUCED: boundary scenario violates the ordering contract.'
