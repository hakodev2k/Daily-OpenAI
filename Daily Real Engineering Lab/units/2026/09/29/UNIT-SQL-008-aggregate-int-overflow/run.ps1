$ErrorActionPreference = "Stop"
$unitRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$server = $env:LAB_SQL_SERVER

if ([string]::IsNullOrWhiteSpace($server)) {
    $server = "(localdb)\MSSQLLocalDB"
}

& sqlcmd -S $server -E -b -i "$unitRoot/starter/query.sql"

if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}
