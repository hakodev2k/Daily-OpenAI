$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
& "$PSScriptRoot/setup.ps1" | Out-Null
$reportOut = Join-Path $env:TEMP 'sql001-verify-report.txt'
$writerOut = Join-Path $env:TEMP 'sql001-verify-writer.txt'
$report = Start-Process sqlcmd -ArgumentList @('-S','localhost','-E','-b','-i',"$root/starter/session-a-report.sql",'-o',$reportOut) -PassThru
Start-Sleep -Milliseconds 250
$writer = Start-Process sqlcmd -ArgumentList @('-S','localhost','-E','-b','-i',"$root/starter/session-b-writer.sql",'-o',$writerOut) -PassThru
$report.WaitForExit(); $writer.WaitForExit()
if ($report.ExitCode -ne 0 -or $writer.ExitCode -ne 0) { throw 'A SQL session failed.' }
$result = Get-Content $reportOut -Raw
$writerResult = Get-Content $writerOut -Raw
if ($result -match '2\s+300\.00' -and $writerResult -match 'WRITER_COMMITTED') {
    Write-Host 'VERIFY_PASS: report is transaction-consistent and writer completed.'
    exit 0
}
Write-Error 'VERIFY_FAIL: expected one transaction-consistent report view while preserving writer completion.'
exit 1