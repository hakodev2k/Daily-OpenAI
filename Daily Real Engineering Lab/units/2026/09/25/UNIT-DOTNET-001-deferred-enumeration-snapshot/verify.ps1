$ErrorActionPreference='Stop'
dotnet run --project ./starter/Lab.csproj
if($LASTEXITCODE -ne 0){throw 'Learner starter still violates batch consistency.'}
Write-Host 'Verification passed.'