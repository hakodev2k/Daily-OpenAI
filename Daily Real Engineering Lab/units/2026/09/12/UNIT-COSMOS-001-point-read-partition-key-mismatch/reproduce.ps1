$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/CosmosPartitionLab.csproj" -- known
$output | Write-Host
if ($output -notmatch 'STATUS=404\|ID=case-1042') {
    throw 'Starter symptom was not reproduced: expected the known ticket to return 404.'
}
Write-Host 'REPRODUCED: known item exists but point read returned 404.'