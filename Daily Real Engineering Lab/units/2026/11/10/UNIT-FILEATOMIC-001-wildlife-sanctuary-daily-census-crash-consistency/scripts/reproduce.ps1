$ErrorActionPreference = 'Stop'
$output = & dotnet run --project "$PSScriptRoot/../starter/CensusLab.csproj" --configuration Release 2>&1
$exitCode = $LASTEXITCODE
$output | ForEach-Object { Write-Output $_ }
if ($exitCode -eq 1 -and ($output -match 'INJECTED_FAULT=observed') -and ($output -match 'OLD_REPORT_PRESERVED=false') -and ($output -match 'CONTRACT=FAIL')) { Write-Output 'REPRODUCE=PASS (expected starter failure)'; exit 0 }
throw "Expected original failure not reproduced; exit=$exitCode"
