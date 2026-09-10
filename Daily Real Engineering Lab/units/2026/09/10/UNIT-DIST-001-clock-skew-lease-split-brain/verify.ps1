$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$output = dotnet run --project "$root/starter/LeaseLab.csproj" -- --verify
$output | Write-Host
if ($LASTEXITCODE -ne 0 -or $output -notmatch 'LAB_VERIFY_PASS') {
    throw 'Verification failed. The learner-editable starter still violates the lease invariant or broke valid expiry takeover.'
}
Write-Host 'VERIFY_PASS'
