$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/GardenImport.csproj'
$output = & dotnet run --project $project
if ($LASTEXITCODE -ne 0) { throw 'Learner code failed' }
$output | ForEach-Object { Write-Output $_ }
if (($output -match '^DuplicateRejected=True$').Count -ne 1) { throw 'Must reject case-insensitive duplicate keys explicitly' }
if (($output -match '^UniqueBatchStored=3$').Count -ne 1) { throw 'Must preserve all three unique rows' }
if (($output -match '^OriginalBatchCommitted=False$').Count -ne 1) { throw 'Conflicted batch must not be committed' }
Write-Output 'PASS: duplicate rejected, original batch not committed, unique batch intact.'
