$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$output = & dotnet run --project (Join-Path $root 'starter/BulkLab.csproj') --configuration Release 2>&1
$exitCode = $LASTEXITCODE
$output | ForEach-Object { Write-Output $_ }
if ($exitCode -ne 0 -or -not (($output | Out-String) -match 'CHECK_PASSED')) {
    throw 'Learner starter did not satisfy business invariants.'
}
Write-Output 'VERIFIED_LEARNER_STARTER'
