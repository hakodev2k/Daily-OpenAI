$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$output = & dotnet run --project "$root/starter/BulkInspector.csproj" 2>&1
$exit = $LASTEXITCODE
$text = $output -join "`n"
if ($exit -eq 0) { throw "Verification failed: learner-editable starter still reports success for a partial-failure fixture.`n$text" }
if ($text -notmatch 'FAILED_ITEMS=2') { throw "Verification failed: expected learner code to report exactly FAILED_ITEMS=2.`n$text" }
if ($text -notmatch 'SKU-101' -or $text -notmatch 'SKU-103') { throw "Verification failed: failed document identifiers must be observable.`n$text" }
Write-Host "Verified: learner code detects and surfaces both failed bulk operations."
