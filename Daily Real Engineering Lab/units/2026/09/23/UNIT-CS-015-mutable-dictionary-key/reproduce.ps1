$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/Starter.csproj" 2>&1 | Out-String
Write-Host $output
if ($LASTEXITCODE -ne 0 -or $output -notmatch 'REPRODUCED') { throw 'Starter did not reproduce the expected symptom.' }
Write-Host 'Reproduction confirmed.'