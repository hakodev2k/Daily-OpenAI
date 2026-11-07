$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/FreightContracts.csproj'
dotnet run --project $project -- --self-test
$code = $LASTEXITCODE
if ($code -ne 0) {
    Write-Error 'Verification failed; fix starter/ManifestIngress.cs without weakening tests.'
    exit 1
}
Write-Host 'VERIFIED: all contract cases match policy.'
exit 0
