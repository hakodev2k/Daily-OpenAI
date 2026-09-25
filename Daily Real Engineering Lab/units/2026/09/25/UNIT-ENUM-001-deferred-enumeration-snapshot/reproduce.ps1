$ErrorActionPreference='Stop'
dotnet run --project ./starter/Lab.csproj
if($LASTEXITCODE -eq 0){throw 'Expected inconsistency was not reproduced.'}
Write-Host 'Reproduction confirmed.'