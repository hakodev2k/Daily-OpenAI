$ErrorActionPreference = "Stop"
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
& (Join-Path $here "run.ps1") -Project starter
if ($LASTEXITCODE -eq 0 -and $?) { throw "Starter unexpectedly passed" }
Write-Host "PASS: original starter failure reproduced"
