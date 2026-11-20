$ErrorActionPreference = 'Stop'
$output = @(dotnet run --project (Join-Path $PSScriptRoot '../starter/GrantPolicyLab.csproj') 2>&1)
if ($LASTEXITCODE -ne 0) { throw 'Starter process failed unexpectedly' }
$text = $output -join "`n"
if ($text -notmatch 'initial: InvalidOperationException' -or $text -notmatch 'recovery: InvalidOperationException') {
  throw "Original failure not reproduced. Output:`n$text"
}
Write-Output 'REPRODUCED: initial and recovery both failed'
