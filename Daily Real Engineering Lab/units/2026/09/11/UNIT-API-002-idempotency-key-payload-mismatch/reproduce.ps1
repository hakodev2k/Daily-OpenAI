$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$output = dotnet run --project (Join-Path $root 'starter/IdempotencyLab.csproj') | Out-String

Write-Host $output

if ($output -notmatch 'mismatch=status:200,order:ORDER-100') {
    throw 'Expected starter symptom was not reproduced.'
}

if ($output -notmatch 'providerCalls=1') {
    throw 'Expected provider call count was not observed.'
}

Write-Host 'Reproduction confirmed: the second logical request received the prior response.'
