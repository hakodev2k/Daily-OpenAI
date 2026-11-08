$root = Split-Path $PSScriptRoot -Parent
$project = Join-Path $root 'starter/Lab.csproj'
$output = & dotnet run --project $project 2>&1
$code = $LASTEXITCODE
$output | ForEach-Object { Write-Output $_ }
if ($code -ne 2 -or (($output -join ' ') -notmatch 'count=1; before=True; after=False')) { Write-Error 'Starter symptom not reproduced'; exit 1 }
Write-Output 'Reproduced expected starter failure (exit 2).'
exit 2
