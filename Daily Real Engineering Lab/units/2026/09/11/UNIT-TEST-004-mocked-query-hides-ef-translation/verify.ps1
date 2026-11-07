$ErrorActionPreference = 'Stop'
$project = "$PSScriptRoot/starter/Lab.csproj"
$output = & dotnet run --project $project -- compare 2>&1 | Out-String
if ($LASTEXITCODE -ne 0) {
  Write-Error "Learner implementation still fails. Output: $output"
}
if ($output -notmatch 'memory-count=1') {
  Write-Error "Memory behavior regressed. Output: $output"
}
if ($output -notmatch 'sqlite-count=1') {
  Write-Error "Relational provider behavior is not fixed. Output: $output"
}
if ($output -match 'sqlite-error=') {
  Write-Error "Relational path still reports an error. Output: $output"
}
Write-Host 'VERIFIED: learner-editable starter preserves behavior and executes successfully through the relational provider.'