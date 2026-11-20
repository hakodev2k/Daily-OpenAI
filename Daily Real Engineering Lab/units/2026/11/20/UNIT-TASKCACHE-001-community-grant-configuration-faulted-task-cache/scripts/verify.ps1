$ErrorActionPreference = 'Stop'
$output = @(dotnet run --project (Join-Path $PSScriptRoot '../starter/GrantPolicyLab.csproj') 2>&1)
if ($LASTEXITCODE -ne 0) { throw 'Learner starter failed to run' }
$text = $output -join "`n"
if ($text -notmatch 'initial: InvalidOperationException' -or $text -notmatch 'recovery: grant-policy:v2') {
  throw "VERIFY FAILED (learner starter):`n$text"
}
Write-Output 'VERIFY PASSED: transient failure followed by recovery'
