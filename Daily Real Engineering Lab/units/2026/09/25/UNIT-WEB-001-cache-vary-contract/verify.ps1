$ErrorActionPreference='Stop'
dotnet run --project ./starter/WebLab.csproj
if($LASTEXITCODE -ne 0){throw 'Variant correctness still fails.'}
Write-Host 'Verification passed.'