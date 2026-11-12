$ErrorActionPreference = "Stop"
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
& (Join-Path $here "run.ps1") -Project starter
Write-Host "PASS: learner-modified starter verified"
