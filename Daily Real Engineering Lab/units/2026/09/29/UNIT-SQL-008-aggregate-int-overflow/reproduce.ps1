$ErrorActionPreference = "Stop"
$unitRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$server = $env:LAB_SQL_SERVER

if ([string]::IsNullOrWhiteSpace($server)) {
    $server = "(localdb)\MSSQLLocalDB"
}

& "$unitRoot/setup.ps1"

$output = (& sqlcmd -S $server -E -b -r1 -i "$unitRoot/starter/query.sql" 2>&1 | Out-String)
$exitCode = $LASTEXITCODE
Write-Host $output

if ($exitCode -eq 0) {
    throw "Starter query unexpectedly succeeded; intended failure was not reproduced."
}

if ($output -notmatch "8115") {
    throw "Query failed, but SQL Server error 8115 was not observed."
}

Write-Host "REPRODUCE=PASS"
