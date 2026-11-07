$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$output = & dotnet run --project (Join-Path $root 'starter/BulkLab.csproj') --configuration Release 2>&1
$exitCode = $LASTEXITCODE
$output | ForEach-Object { Write-Output $_ }
if ($exitCode -eq 0 -or -not (($output | Out-String) -match 'CHECK_FAILED')) {
    throw 'Expected starter business invariant failure was not reproduced.'
}
Write-Output 'REPRODUCED_EXPECTED_FAILURE'
