$ErrorActionPreference = "Stop"
$output = dotnet run --project "$PSScriptRoot/starter/TimeoutOrphanWorkLab.csproj" 2>&1 | Out-String
Write-Host $output
if ($LASTEXITCODE -ne 0) {
  throw "Learner-editable starter still exits with failure."
}
if ($output -notmatch "NO_ORPHANED_WORK") {
  throw "Expected NO_ORPHANED_WORK after the fix."
}
if ($output -match "ORPHANED_WORK_DETECTED") {
  throw "Timed-out work still crosses the test boundary."
}
Write-Host "Verification passed."
