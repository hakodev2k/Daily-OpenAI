$ErrorActionPreference = 'Stop'
$known = dotnet run --project "$PSScriptRoot/starter/CosmosPartitionLab.csproj" -- known
$unknown = dotnet run --project "$PSScriptRoot/starter/CosmosPartitionLab.csproj" -- unknown
$known | Write-Host
$unknown | Write-Host
if ($known -notmatch 'STATUS=200\|ID=case-1042\|PK=tenant-acme') {
    throw 'Verification failed: learner-edited starter still cannot point-read the known ticket.'
}
if ($unknown -notmatch 'STATUS=404\|ID=case-9999') {
    throw 'Verification failed: unknown ticket must remain NotFound.'
}
Write-Host 'VERIFIED: canonical address reads the known item and preserves NotFound behavior.'