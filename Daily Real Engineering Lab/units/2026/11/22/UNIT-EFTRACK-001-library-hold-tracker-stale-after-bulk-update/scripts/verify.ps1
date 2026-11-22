$ErrorActionPreference = 'Stop'
$output = & dotnet run --project (Join-Path $PSScriptRoot '../starter/LibraryHoldLab.csproj') 2>&1
if ($LASTEXITCODE -ne 0) { throw "Run failed: $output" }
$text = $output -join "`n"
Write-Output $text
foreach ($line in @('AFFECTED=1','DATABASE=Approved','RESPONSE=Approved')) {
    if ($text -notmatch "(?m)^$line\r?$") { throw "VERIFY FAILED: missing $line" }
}
Write-Output 'VERIFY PASS'
