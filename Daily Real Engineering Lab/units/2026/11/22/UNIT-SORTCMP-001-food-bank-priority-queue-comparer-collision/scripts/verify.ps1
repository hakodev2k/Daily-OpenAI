$ErrorActionPreference='Stop'
$project=Join-Path $PSScriptRoot '../starter/DispatchLab.csproj'
$output=dotnet run --project $project -- --verify 2>&1 | Out-String
Write-Output $output
if($LASTEXITCODE -ne 0){throw 'Verification failed: request loss detected'}
if($output -notmatch 'EXPECTED=4 ACTUAL=4'){throw 'Count contract failed'}
$lines=@($output -split "`r?`n" | Where-Object {$_ -match '^DISPATCH '})
if($lines.Count -ne 4){throw 'Dispatch count failed'}
if(($lines -join '|') -ne 'DISPATCH A-101|DISPATCH B-205|DISPATCH D-410|DISPATCH C-300'){throw 'Ordering contract failed'}
Write-Output 'PASS: cardinality, uniqueness and priority order'
