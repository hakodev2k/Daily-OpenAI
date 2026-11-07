$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/Starter.csproj" -- --verify 2>&1 | Out-String
Write-Host $output
if ($LASTEXITCODE -ne 0 -or $output -notmatch 'VERIFY_OK') { throw 'Learner-editable starter still violates the required lookup behavior.' }
Write-Host 'Verification passed.'