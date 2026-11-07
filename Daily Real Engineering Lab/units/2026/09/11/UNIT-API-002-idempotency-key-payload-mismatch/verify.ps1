$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$output = dotnet run --project (Join-Path $root 'starter/IdempotencyLab.csproj') | Out-String

Write-Host $output

if ($output -notmatch 'first=status:200,order:ORDER-100') {
    throw 'First request behavior regressed.'
}

if ($output -notmatch 'retry=status:200,order:ORDER-100,receipt:rcpt-001') {
    throw 'Same-request retry is not returning the original successful result.'
}

if ($output -notmatch 'mismatch=status:409,order:ORDER-200') {
    throw 'Different request reusing the key must be rejected explicitly.'
}

if ($output -notmatch 'providerCalls=1') {
    throw 'Provider must not receive a duplicate or mismatched charge.'
}

Write-Host 'Verification passed: retry is idempotent and key reuse across different requests is rejected.'
