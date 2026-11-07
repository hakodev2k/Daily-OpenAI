$ErrorActionPreference = "Stop"
$unitRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$server = $env:LAB_SQL_SERVER

if ([string]::IsNullOrWhiteSpace($server)) {
    $server = "(localdb)\MSSQLLocalDB"
}

& "$unitRoot/setup.ps1"

$output = (& sqlcmd -S $server -E -b -h -1 -W -i "$unitRoot/starter/query.sql" 2>&1 | Out-String)
$exitCode = $LASTEXITCODE
Write-Host $output

if ($exitCode -ne 0) {
    throw "Learner query still fails."
}

if ($output -notmatch "(?m)^\s*3300000000\s*$") {
    throw "Expected TotalCents=3300000000 was not returned."
}

Write-Host "VERIFY=PASS"
