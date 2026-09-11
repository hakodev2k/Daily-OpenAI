$ErrorActionPreference = 'Stop'
$project = "$PSScriptRoot/starter/Lab.csproj"
$memory = & dotnet run --project $project -- memory 2>&1 | Out-String
if ($LASTEXITCODE -ne 0 -or $memory -notmatch 'memory-count=1') {
  Write-Error "Test-like path did not produce the expected baseline. Output: $memory"
}
$sqlite = & dotnet run --project $project -- sqlite 2>&1 | Out-String
if ($sqlite -notmatch 'sqlite-error=InvalidOperationException') {
  Write-Error "Expected relational translation failure was not reproduced. Output: $sqlite"
}
Write-Host 'REPRODUCED: test-like IQueryable path passes while relational provider path fails.'