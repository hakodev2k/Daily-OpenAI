$ErrorActionPreference = 'Stop'
$unit = Split-Path $PSScriptRoot -Parent
$out = & dotnet run --project (Join-Path $unit 'starter/ProbeLab.csproj') 2>&1 | Out-String
Write-Output $out
if ($out -notmatch 'SUMMARY restarts=2 rejected=2 accepted=2 lost=3') { throw 'Starter symptom not reproduced' }
Write-Output 'REPRODUCED: dependency outage caused unnecessary restarts and lost in-flight work.'
