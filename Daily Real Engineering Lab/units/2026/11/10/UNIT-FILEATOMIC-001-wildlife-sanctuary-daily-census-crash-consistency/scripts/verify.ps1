$ErrorActionPreference = 'Stop'
$output = & dotnet run --project "$PSScriptRoot/../starter/CensusLab.csproj" --configuration Release 2>&1
$exitCode = $LASTEXITCODE
$output | ForEach-Object { Write-Output $_ }
if ($exitCode -eq 0 -and ($output -match 'INJECTED_FAULT=observed') -and ($output -match 'OLD_REPORT_PRESERVED=true') -and ($output -match 'NEW_REPORT_COMMITTED=true') -and ($output -match 'NO_ORPHAN_TEMP_FILES=true') -and ($output -match 'CONTRACT=PASS')) { Write-Output 'VERIFY=PASS'; exit 0 }
throw "Starter still violates recovery contract; exit=$exitCode"
