$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
& "$PSScriptRoot/setup.ps1" | Out-Null
$reportOut = Join-Path $env:TEMP 'sql001-report.txt'
$writerOut = Join-Path $env:TEMP 'sql001-writer.txt'
$report = Start-Process sqlcmd -ArgumentList @('-S','localhost','-E','-b','-i',"$root/starter/session-a-report.sql",'-o',$reportOut) -PassThru
Start-Sleep -Milliseconds 250
$writer = Start-Process sqlcmd -ArgumentList @('-S','localhost','-E','-b','-i',"$root/starter/session-b-writer.sql",'-o',$writerOut) -PassThru
$report.WaitForExit(); $writer.WaitForExit()
if ($report.ExitCode -ne 0 -or $writer.ExitCode -ne 0) { throw 'A SQL session failed.' }
$result = Get-Content $reportOut -Raw
$writerResult = Get-Content $writerOut -Raw
$result; $writerResult
if ($result -notmatch '2\s+600\.00') { throw 'Expected inconsistency was not reproduced.' }
if ($writerResult -notmatch 'WRITER_COMMITTED') { throw 'Writer did not complete.' }
Write-Host 'REPRODUCED: report combined values from different committed states.'