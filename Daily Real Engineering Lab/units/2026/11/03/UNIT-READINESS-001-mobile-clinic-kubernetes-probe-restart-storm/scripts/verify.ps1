$ErrorActionPreference = 'Stop'
$unit = Split-Path $PSScriptRoot -Parent
$out = & dotnet run --project (Join-Path $unit 'starter/ProbeLab.csproj') 2>&1 | Out-String
Write-Output $out
if ($LASTEXITCODE -ne 0 -or $out -notmatch 'SUMMARY restarts=0 rejected=2 accepted=2 lost=0') { throw 'Verification failed: inspect learner-edited starter code' }
if ($out -notmatch 'minute=1 db=False live=True ready=False' -or $out -notmatch 'minute=3 db=True live=True ready=True') { throw 'Probe semantics regression' }
Write-Output 'PASS: learner starter meets probe policy contract.'
