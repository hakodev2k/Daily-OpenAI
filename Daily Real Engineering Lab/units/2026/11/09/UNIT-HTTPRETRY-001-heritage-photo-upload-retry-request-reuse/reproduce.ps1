$ErrorActionPreference = 'Stop'
$output = & dotnet run --project (Join-Path $PSScriptRoot 'starter/HeritageUpload.csproj') 2>&1
$text = $output -join "`n"
Write-Output $text
if ($text -match 'ERROR=InvalidOperationException; RECEIVED=1') { exit 0 }
Write-Error 'Starter failure was not reproduced as expected'
exit 1
