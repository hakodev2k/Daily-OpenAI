$ErrorActionPreference='Stop'
dotnet run --project ./starter/Starter.csproj -- --reproduce
if($LASTEXITCODE -ne 0){throw 'Expected symptom not reproduced.'}
Write-Host 'REPRODUCED'