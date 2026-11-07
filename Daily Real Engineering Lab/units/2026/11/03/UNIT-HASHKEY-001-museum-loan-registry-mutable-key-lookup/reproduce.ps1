$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/LoanRegistry.csproj"
$output = & dotnet run --project $project 2>&1
$exit = $LASTEXITCODE
$output | ForEach-Object { Write-Host $_ }
if ($exit -ne 1 -or (($output -join "`n") -notmatch "After correction: found=False") -or (($output -join "`n") -notmatch "count=1")) { throw "Starter symptom not reproduced (exit=$exit)." }
Write-Host "PASS: intended starter failure reproduced."
