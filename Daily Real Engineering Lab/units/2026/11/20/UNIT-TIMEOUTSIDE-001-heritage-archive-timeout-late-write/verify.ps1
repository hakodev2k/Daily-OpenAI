$ErrorActionPreference = 'Stop'
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
$output = & dotnet run --project (Join-Path $here 'starter/ArchiveLab.csproj') 2>&1 | Out-String
if ($LASTEXITCODE -ne 0) { throw $output }
Write-Output $output
foreach ($expected in @('TIMEOUT slow','AT_RESPONSE 0','AFTER_WAIT 0','OK fast','FINAL 1')) {
 if (-not $output.Contains($expected)) { throw "Verification failed: missing $expected" }
}
if ($output.Contains('WRITE slow')) { throw 'Late write detected' }
Write-Output 'VERIFIED: learner workspace passes behavioral assertions'
