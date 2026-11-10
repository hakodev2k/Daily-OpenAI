$ErrorActionPreference = 'Stop'
$unit = Split-Path $PSScriptRoot -Parent
$output = & dotnet run --project (Join-Path $unit 'tests/Contract/Contract.csproj') 2>&1
$code = $LASTEXITCODE
$output | ForEach-Object { Write-Output $_ }
if ($code -eq 0) { throw 'Starter unexpectedly passes.' }
if (-not (($output -join "`n") -match 'DST actual=8 expected=9')) { throw 'Unexpected failure; intended symptom absent.' }
Write-Output 'REPRODUCED'
