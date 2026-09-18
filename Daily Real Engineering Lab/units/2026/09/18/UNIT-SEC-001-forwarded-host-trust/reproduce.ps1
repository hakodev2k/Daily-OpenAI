$ErrorActionPreference='Continue'
dotnet run --project ./starter/ForwardedHostLab.csproj -- reproduce
if ($LASTEXITCODE -eq 2) { Write-Host 'Expected symptom reproduced.'; exit 0 }
Write-Error 'Expected symptom was not reproduced.'
exit 1