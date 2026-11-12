$ErrorActionPreference = "Stop"
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
& (Join-Path $here "run.ps1") -Project starter
if (-not $?) { throw "Learner starter verification failed" }
