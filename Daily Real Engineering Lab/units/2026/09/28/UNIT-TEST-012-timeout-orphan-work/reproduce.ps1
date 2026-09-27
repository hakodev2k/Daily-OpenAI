$ErrorActionPreference = "Stop"
$output = dotnet run --project "$PSScriptRoot/starter/TimeoutOrphanWorkLab.csproj" 2>&1 | Out-String
Write-Host $output
if ($output -notmatch "ORPHANED_WORK_DETECTED") {
  throw "Starter symptom was not reproduced. Expected ORPHANED_WORK_DETECTED."
}
Write-Host "Reproduction confirmed: work from the timed-out scenario crossed the test boundary."
