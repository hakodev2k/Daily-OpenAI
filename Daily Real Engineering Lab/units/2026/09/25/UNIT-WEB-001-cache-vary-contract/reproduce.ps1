$ErrorActionPreference='Stop'
dotnet run --project ./starter/WebLab.csproj
if($LASTEXITCODE -eq 0){throw 'Expected starter to reproduce mismatch.'}
Write-Host 'Reproduction confirmed.'