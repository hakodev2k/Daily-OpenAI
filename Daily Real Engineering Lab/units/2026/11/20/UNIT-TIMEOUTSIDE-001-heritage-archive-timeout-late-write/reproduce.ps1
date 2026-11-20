$ErrorActionPreference = 'Stop'
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
$output = & dotnet run --project (Join-Path $here 'starter/ArchiveLab.csproj') 2>&1 | Out-String
if ($LASTEXITCODE -ne 0) { throw $output }
Write-Output $output
foreach ($expected in @('TIMEOUT slow','AT_RESPONSE 0','AFTER_WAIT 1','OK fast','FINAL 2')) {
 if (-not $output.Contains($expected)) { throw "Expected starter symptom missing: $expected" }
}
Write-Output 'REPRODUCED: late side effect'
