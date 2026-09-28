$ErrorActionPreference = "Stop"
$unitRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$server = $env:LAB_SQL_SERVER

if ([string]::IsNullOrWhiteSpace($server)) {
    $server = "(localdb)\MSSQLLocalDB"
}

if (-not (Get-Command sqlcmd -ErrorAction SilentlyContinue)) {
    throw "sqlcmd was not found in PATH."
}

& sqlcmd -S $server -E -b -i "$unitRoot/starter/setup.sql"

if ($LASTEXITCODE -ne 0) {
    throw "Database setup failed with exit code $LASTEXITCODE."
}

Write-Host "SETUP=PASS"
