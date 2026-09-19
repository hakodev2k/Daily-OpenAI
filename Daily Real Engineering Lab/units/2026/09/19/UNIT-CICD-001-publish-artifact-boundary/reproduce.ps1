$ErrorActionPreference = 'Continue'
dotnet run --project ./starter/ArtifactLab.csproj -- reproduce
if ($LASTEXITCODE -eq 2) { Write-Host 'Expected artifact drift reproduced.'; exit 0 }
Write-Error 'Expected artifact drift was not reproduced.'
exit 1
