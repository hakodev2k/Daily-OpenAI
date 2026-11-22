$ErrorActionPreference = 'Stop'
$output = & dotnet run --project (Join-Path $PSScriptRoot '../starter/LibraryHoldLab.csproj') 2>&1
if ($LASTEXITCODE -ne 0) { throw "Run failed: $output" }
$text = $output -join "`n"
Write-Output $text
if ($text -notmatch '(?m)^DATABASE=Approved\r?$' -or $text -notmatch '(?m)^RESPONSE=Pending\r?$') { throw 'Original failure not reproduced; if already fixed, run verify.ps1' }
Write-Output 'REPRODUCED: database and response disagree'
