$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/LoanRegistry.csproj"
$output = & dotnet run --project $project 2>&1
$exit = $LASTEXITCODE
$output | ForEach-Object { Write-Host $_ }
if ($exit -ne 0 -or (($output -join "`n") -notmatch "After correction: found=True") -or (($output -join "`n") -notmatch "status=APPROVED") -or (($output -join "`n") -notmatch "count=1")) { throw "Learner starter has not satisfied the correction contract (exit=$exit)." }
Write-Host "PASS: learner code meets observable correction contract."
