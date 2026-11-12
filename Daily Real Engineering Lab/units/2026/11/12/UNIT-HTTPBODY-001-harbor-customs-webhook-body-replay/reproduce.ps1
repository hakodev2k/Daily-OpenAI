$ErrorActionPreference = "Stop"
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
try {
  & (Join-Path $here "run.ps1") -Project starter
  throw "Starter unexpectedly passed: no reproduction"
} catch {
  if ($_.Exception.Message -like "Starter unexpectedly passed*") { throw }
  if ($_.Exception.Message -notlike "Expected 200, got 400*") { throw }
  Write-Host "PASS: original starter returned HTTP 400 as expected"
}
