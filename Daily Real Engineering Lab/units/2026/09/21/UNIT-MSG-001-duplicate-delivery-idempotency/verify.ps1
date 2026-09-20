$ErrorActionPreference='Stop'
dotnet run --project ./starter/Starter.csproj -- --verify
if($LASTEXITCODE -ne 0){throw 'VERIFY_FAIL'}
Write-Host 'VERIFY_PASS'