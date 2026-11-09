$ErrorActionPreference = 'Stop'
$output = & dotnet run --project (Join-Path $PSScriptRoot 'starter/HeritageUpload.csproj') 2>&1
$text = $output -join "`n"
Write-Output $text
if ($LASTEXITCODE -eq 0 -and $text -match 'RESULT=200; RECEIVED=2' -and $text -notmatch 'ERROR=') { exit 0 }
Write-Error 'Learner starter did not satisfy the retry contract'
exit 1
