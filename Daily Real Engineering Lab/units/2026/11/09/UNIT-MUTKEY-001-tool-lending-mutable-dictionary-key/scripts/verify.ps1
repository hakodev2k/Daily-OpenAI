$root = Split-Path $PSScriptRoot -Parent
$project = Join-Path $root 'starter/Lab.csproj'
$output = & dotnet run --project $project 2>&1
$code = $LASTEXITCODE
$output | ForEach-Object { Write-Output $_ }
if ($code -ne 0 -or (($output -join ' ') -notmatch 'count=1; before=True; after=True')) { Write-Error 'Learner starter verification failed'; exit 1 }
Write-Output 'PASS: reservation remains retrievable after display-code edit.'
exit 0
